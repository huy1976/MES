namespace AIChatbot.Api.Models;

public record QueryRequest(string Question, int? TopK);
public record QueryHit(string Id, float Score, string Text, string? Source);
public record QueryResponse(string Answer, List<QueryHit> Contexts);

