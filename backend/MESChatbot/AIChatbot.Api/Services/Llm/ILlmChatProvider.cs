namespace AIChatbot.Api.Services.Llm;

public interface ILlmChatProvider
{
    Task<string> AskAsync(string system, string user, CancellationToken ct = default);
}
