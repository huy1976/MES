using System.Text.Json;
using AIChatbot.Api.Services.Database;
using OpenAI.Chat;

namespace AIChatbot.Api.Services.Llm;

public class OpenAIChatProvider : ILlmChatProvider
{
    private readonly ChatClient _chatClient;
    private readonly ISqlDatabaseService _dbService;

    // Inject thêm ISqlDatabaseService vào constructor
    public OpenAIChatProvider(ChatClient chatClient, ISqlDatabaseService dbService)
    {
        _chatClient = chatClient;
        _dbService = dbService;
    }

    public async Task<string> AskAsync(string system, string user, CancellationToken cancellationToken = default)
    {
        // 1. Lấy lược đồ (schema) của database để AI biết cấu trúc các bảng
        var schema = await _dbService.GetDatabaseSystemPromptAsync();

        // Thêm schema vào system prompt để AI biết cách viết SQL chính xác
        var fullSystemPrompt = $"{system}\n\nDưới đây là cấu trúc SQL Database của hệ thống, hãy dùng nó để viết câu lệnh T-SQL khi cần thiết:\n{schema}";

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(fullSystemPrompt),
            new UserChatMessage(user)
        };

        // 2. Định nghĩa công cụ (Tool) cho AI
        var querySqlTool = ChatTool.CreateFunctionTool(
            functionName: "QuerySqlDatabase",
            functionDescription: "Thực thi câu lệnh T-SQL để truy vấn dữ liệu từ cơ sở dữ liệu SQL Server. Công cụ này trả về kết quả dưới dạng chuỗi.",
            functionParameters: BinaryData.FromString(@"
            {
                ""type"": ""object"",
                ""properties"": {
                    ""sqlQuery"": {
                        ""type"": ""string"",
                        ""description"": ""Câu lệnh T-SQL hợp lệ để thực thi. Ví dụ: SELECT TOP 5 * FROM Users""
                    }
                },
                ""required"": [""sqlQuery""]
            }")
        );

        var options = new ChatCompletionOptions
        {
            Tools = { querySqlTool }
        };

        // 3. Vòng lặp xử lý hội thoại và Tool Calls
        while (true)
        {
            var completion = await _chatClient.CompleteChatAsync(messages, options, cancellationToken);
            var response = completion.Value;

            // Lưu lại phản hồi của Assistant (kể cả khi nó là văn bản thường hay là yêu cầu gọi Tool)
            messages.Add(new AssistantChatMessage(response));

            // Kiểm tra xem AI có muốn gọi Tool không
            if (response.FinishReason == ChatFinishReason.ToolCalls)
            {
                foreach (var toolCall in response.ToolCalls)
                {
                    if (toolCall.FunctionName == "QuerySqlDatabase")
                    {
                        // Extract tham số sqlQuery mà AI sinh ra
                        using var doc = JsonDocument.Parse(toolCall.FunctionArguments);
                        var sqlQuery = doc.RootElement.GetProperty("sqlQuery").GetString() ?? string.Empty;

                        Console.WriteLine($"[Tool Called] AI đang chạy SQL: {sqlQuery}");

                        // Gọi service chạy SQL thực tế dưới database
                        var queryResult = await _dbService.ExecuteQueryAsync(sqlQuery);

                        // Đưa kết quả từ database quay lại cho AI đọc
                        messages.Add(new ToolChatMessage(toolCall.Id, queryResult));
                    }
                }
                // Vòng lặp sẽ tiếp tục, gọi API OpenAI lần nữa với kết quả từ database
            }
            else
            {
                // Nếu AI không gọi tool nữa (FinishReason == Stop), trả về kết quả cuối cùng
                return response.Content.Count > 0
                    ? response.Content[0].Text
                    : string.Empty;
            }
        }
    }
}