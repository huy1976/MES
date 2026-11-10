using AIChatbot.Api.Models;
using AIChatbot.Api.Services.Embedding;
using AIChatbot.Api.Services.Llm;
using AIChatbot.Api.Services.Vecto;
using System.Text;

namespace AIChatbot.Api.Services;

public class RagPipeline
{
    private readonly IEmbeddingProvider _embeddingProvider;
    private readonly ILlmChatProvider _llmProvider;
    private readonly IQdrantClient _vectorClient;
    private readonly AppConfig _appConfig;

    public RagPipeline(IEmbeddingProvider embeddingProvider,
        ILlmChatProvider llmProvider, IQdrantClient vectorClient, AppConfig appConfig)
    {
        _embeddingProvider = embeddingProvider;
        _llmProvider = llmProvider;
        _vectorClient = vectorClient;
        _appConfig = appConfig;
    }

    public async Task IngestAsync(IEnumerable<(string text, string? source)> chunks, CancellationToken ct = default)
    {
        int dim = await _embeddingProvider.GetDimAsync(ct);
        await _vectorClient.EnsureCollectionAsync(dim, ct);

        var list = new List<VecPoint>();
        foreach (var (text, source) in chunks)
        {
            var vec = await _embeddingProvider.EmbedAsync(text, ct);
            list.Add(new VecPoint(Guid.NewGuid().ToString("N"), vec, text, source));
        }
        await _vectorClient.UpsertAsync(list, ct);
    }

    // SỬA ĐỔI: Thêm tham số List<ChatMessage> history vào hàm
    public async Task<QueryResponse> AskAsync(string question, List<ChatMessage> history, int? topK, CancellationToken cancellationToken = default)
    {
        int dimension = await _embeddingProvider.GetDimAsync(cancellationToken);
        await _vectorClient.EnsureCollectionAsync(dimension, cancellationToken);

        // Vẫn dùng câu hỏi hiện tại để search Vector DB
        var questionVector = await _embeddingProvider.EmbedAsync(question, cancellationToken);
        var hits = await _vectorClient.SearchAsync(questionVector, topK ?? _appConfig.Rag.TopK, cancellationToken);
        var filteredHits = hits.Where(h => h.Score >= _appConfig.Rag.MinScore).ToList();

        var dedupedHits = new List<VecHit>();
        var citations = string.Empty;
        var context = string.Empty;

        // Build Context từ Vector DB (PDF/TXT)
        if (filteredHits.Count > 0)
        {
            var seenHits = new HashSet<string>();
            foreach (var h in filteredHits)
            {
                var fp = Fingerprint(h.Text);
                if (seenHits.Contains(fp)) continue;
                seenHits.Add(fp);
                dedupedHits.Add(h);
            }

            var numbered = dedupedHits.Select((h, i) => (Idx: i + 1, Hit: h)).ToList();

            // Định dạng lại citation của file PDF cho rõ ràng hơn
            citations = string.Join("\n", numbered.Select(x => $"[#{x.Idx}] File: {x.Hit.Source ?? x.Hit.Id} (Độ tin cậy: {x.Hit.Score:0.000})"));
            context = string.Join("\n---\n", numbered.Select(x => $"ID:{x.Idx}\n{x.Hit.Text}"));
        }

        // System Prompt để ép AI phải trích dẫn tên Bảng SQL
        var system = @"Bạn là trợ lý AI thông minh. Bạn có 2 nguồn thông tin:
1. TÀI LIỆU (PDF/TXT): Sẽ được cung cấp trong phần CONTEXT.
2. CƠ SỞ DỮ LIỆU SQL: Bạn có công cụ truy vấn SQL Server.

QUY TẮC TRÍCH DẪN BẮT BUỘC:
- Nếu bạn lấy dữ liệu từ phần CONTEXT (Tài liệu): Hãy chèn số ID của tài liệu vào ngay trong câu trả lời (ví dụ: '[#1]', '[#2]').
- Nếu bạn lấy dữ liệu bằng cách chạy công cụ SQL: Bạn BẮT BUỘC phải xuống dòng ở cuối câu trả lời và ghi rõ tên bảng theo đúng định dạng sau: '📌 Nguồn SQL: Bảng [Tên Bảng 1], [Tên Bảng 2]'.
- Nếu dùng cả hai nguồn, hãy trích dẫn đầy đủ theo cả 2 quy tắc trên.";

        string contextMessage = string.IsNullOrEmpty(context)
                                ? "Không có tài liệu văn bản nào liên quan."
                                : context;

        // --- SỬA ĐỔI MỚI: XÂY DỰNG NGỮ CẢNH LỊCH SỬ TRÒ CHUYỆN ---
        var conversationContext = new StringBuilder();
        if (history != null && history.Any())
        {
            conversationContext.AppendLine("=== LỊCH SỬ TRÒ CHUYỆN TRƯỚC ĐÓ ===");
            foreach (var msg in history)
            {
                string roleName = msg.Role.ToLower() == "user" ? "Người dùng" : "Trợ lý (Bạn)";
                conversationContext.AppendLine($"{roleName}: {msg.Content}");
            }
            conversationContext.AppendLine("===================================");
        }

        // Gắn lịch sử trò chuyện vào User Prompt
        var user = $"{conversationContext}\n\nCÂU HỎI HIỆN TẠI CỦA NGƯỜI DÙNG:\n{question}\n\nCONTEXT TỪ TÀI LIỆU (Mỗi khối có ID):\n{contextMessage}";

        // Gọi LLM
        var answer = await _llmProvider.AskAsync(system, user, cancellationToken);

        // Chỉ nối trích dẫn PDF nếu câu trả lời của AI thực sự có chứa chuỗi "[#"
        bool aiUsedDocument = answer.Contains("[#");

        if (!string.IsNullOrEmpty(citations) && aiUsedDocument)
        {
            // Thêm phần trích dẫn của file nếu AI có dùng
            answer += "\n\n📄 Nguồn tài liệu đính kèm:\n" + citations;
        }
        else
        {
            // Xóa danh sách dedupedHits để không trả về DTO cho client những file rác mà AI không xài
            dedupedHits.Clear();
        }

        var respHits = dedupedHits.Select(h => new QueryHit(h.Id, h.Score, h.Text, h.Source)).ToList();

        return new QueryResponse(answer, respHits);
    }

    static string Fingerprint(string s)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(s.Trim().ToLowerInvariant());
        return Convert.ToHexString(sha.ComputeHash(bytes)).Substring(0, 16);
    }
}