using OpenAI.Embeddings;

namespace AIChatbot.Api.Services.Embedding;

public class OpenAIEmbeddingProvider : IEmbeddingProvider
{
    private readonly EmbeddingClient _client;
    private int? _dim;

    public OpenAIEmbeddingProvider(EmbeddingClient client)
    {
        _client = client;
    }

    public async Task<float[]> EmbedAsync(string text, CancellationToken ct = default)
    {
        var result = await _client.GenerateEmbeddingAsync(text, cancellationToken: ct);
        var vec = result.Value.ToFloats().ToArray();
        _dim ??= vec.Length;
        return vec;
    }

    public async Task<int> GetDimAsync(CancellationToken ct = default)
    {
        if (_dim.HasValue) return _dim.Value;
        _ = await EmbedAsync("dimension probe", ct);
        return _dim!.Value;
    }
}
