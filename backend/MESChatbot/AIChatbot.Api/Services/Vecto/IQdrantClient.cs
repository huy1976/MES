using System.Security.Cryptography;

namespace AIChatbot.Api.Services.Vecto;

public interface IQdrantClient
{
    Task EnsureCollectionAsync(int vectorSize, CancellationToken ct = default);
    Task UpsertAsync(IEnumerable<VecPoint> points, CancellationToken ct = default);
    Task<List<VecHit>> SearchAsync(float[] query, int topK, CancellationToken ct = default);
}
public record VecPoint(string Id, float[] Vector, string Text, string? Source);
public record VecHit(string Id, float Score, string Text, string? Source);

public class QdrantSearchResponse
{
    public List<QdrantHit> Result { get; set; }
}

public class QdrantHit
{
    public string Id { get; set; }
    public float Score { get; set; }
    public QdrantPayload Payload { get; set; }
}

public class QdrantPayload
{
    public string Text { get; set; }
    public string Source { get; set; }
}