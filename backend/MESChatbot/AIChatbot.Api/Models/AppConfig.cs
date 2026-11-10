namespace AIChatbot.Api.Models;

public class AppConfig
{
    public string Provider { get; set; } = "OpenAI";
    public OpenAIConfig OpenAI { get; set; } = new();
    public QdrantConfig Qdrant { get; set; } = new();
    public RagConfig Rag { get; set; } = new();

}

public class OpenAIConfig 
{
    public string ApiKey { get; set; } = string.Empty;
    public string ChatModel { get; set; } = "gpt-4o-mini";
    public string EmbedModel { get; set; } = "text-embedding-3-small";
}

public class QdrantConfig
{
    public string Endpoint { get; set; } = "http://localhost:6333";
    public string Collection { get; set; } = "docs_basic";
}
public class RagConfig
{
    public int ChunkSize { get; set; } = 800;
    public int ChunkOverlap { get; set; } = 120;
    public int TopK { get; set; } = 3;
    public float MinScore { get; set; } = 0.25f; // gate cho cosine
}
public class ChatMessage
{
    public string Role { get; set; } // "user" hoặc "assistant"
    public string Content { get; set; }
}

// Model hứng tín hiệu JSON từ AI sinh ra (Dùng cho Xuất Excel / Biểu đồ)
public class AiActionRequest
{
    public string Action { get; set; } = string.Empty;    // "ExportLastData", "NewQueryAndExport", "Chart"
    public string Message { get; set; } = string.Empty;   // Lời chào của AI
    public string Query { get; set; } = string.Empty;     // Câu lệnh SQL (nếu có)
    public string ChartType { get; set; } = string.Empty; // "Bar", "Pie", "Line"... (Nếu vẽ biểu đồ)
}
