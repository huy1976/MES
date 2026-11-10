//using AIChatbot.Api.Services.Tools;
//using Microsoft.AspNetCore.Mvc;
//using OpenAI.Chat;
//using System.Text.Json;

//namespace AIChatbot.Api.Controller
//{
//    public record ToolRagChatRequest(string Message, int? TopK, float? MinScore);

//    [ApiController]
//    [Route("chat")]
//    public class RagChatController : ControllerBase
//    {
//        private readonly ChatClient _chat;
//        private readonly SearchDocsTool _search;

//        public RagChatController(ChatClient chat, SearchDocsTool search)
//        {
//            _chat = chat;
//            _search = search;
//        }

//        [HttpPost("assist")]
//        public async Task<IActionResult> Assist(
//            [FromBody] ToolRagChatRequest req,
//            CancellationToken ct
//        )
//        {
//            var searchTool = ChatTool.CreateFunctionTool(
//                functionName: nameof(SearchDocsTool),
//                functionDescription: "Semantic vector search over the knowledge base. Returns relevant passages for the user's query.",
//                functionParameters: BinaryData.FromBytes(
//                    """
//                {
//                  "type":"object",
//                  "properties":{
//                    "query":{"type":"string","description":"User question to search for"},
//                    "topK":{"type":"integer","minimum":1,"maximum":20},
//                    "minScore":{"type":"number","minimum":-1,"maximum":1}
//                  },
//                  "required":["query"],
//                  "additionalProperties": false
//                }
//                """u8.ToArray()
//                )
//            );

//            var messages = new List<ChatMessage>
//            {
//                new SystemChatMessage(
//                    "You are a retrieval-augmented assistant. "
//                        + $"Decide when to call {nameof(SearchDocsTool)}. "
//                        + $"If the user query likely needs facts from our knowledge base, call {nameof(SearchDocsTool)} first, "
//                        + "then answer ONLY using returned snippets. "
//                        + "Cite passages inline with [#ordinal] and list sources at the end. "
//                        + $"If {nameof(SearchDocsTool)} returns no hits, say you don't know."
//                ),
//                new UserChatMessage(req.Message),
//            };

//            var options = new ChatCompletionOptions { Tools = { searchTool } };

//            List<(int Ordinal, string Source, double Score)> lastHits = new();

//            bool requiresAction;
//            do
//            {
//                requiresAction = false;
//                var completionResult = await _chat.CompleteChatAsync(messages, options, ct);
//                var completion = completionResult.Value ?? completionResult;
//                if (completion == null)
//                    return BadRequest("No completion result.");

//                switch (completion.FinishReason)
//                {
//                    case ChatFinishReason.Stop:
//                        messages.Add(new AssistantChatMessage(completion));
//                        var reply =
//                            completion.Content != null && completion.Content.Count > 0
//                                ? completion.Content[0].Text
//                                : string.Empty;
//                        reply += BuildCitations(lastHits);
//                        return Ok(new { reply });

//                    case ChatFinishReason.ToolCalls:
//                        messages.Add(new AssistantChatMessage(completion));
//                        if (completion.ToolCalls != null)
//                        {
//                            foreach (var call in completion.ToolCalls)
//                                await HandleToolCall(call, req, messages, ct, lastHits);
//                        }
//                        requiresAction = true;
//                        break;
//                    default:
//                        throw new NotImplementedException(completion.FinishReason.ToString());
//                }
//            } while (requiresAction);

//            return BadRequest("Unexpected exit from chat loop.");
//        }

//        private static List<(int Ordinal, string Source, double Score)> ParseHits(string resultJson)
//        {
//            var hits = new List<(int, string, double)>();
//            try
//            {
//                using var resultDoc = JsonDocument.Parse(resultJson);
//                hits = resultDoc
//                    .RootElement.GetProperty("hits")
//                    .EnumerateArray()
//                    .Select(h =>
//                        (
//                            Ordinal: h.GetProperty("ordinal").GetInt32(),
//                            Source: h.GetProperty("source").GetString() ?? "(unknown)",
//                            Score: h.GetProperty("score").GetDouble()
//                        )
//                    )
//                    .ToList();
//            }
//            catch
//            { /* ignore parse errors */
//            }
//            return hits;
//        }

//        private static string BuildCitations(List<(int Ordinal, string Source, double Score)> hits)
//        {
//            if (hits.Count == 0)
//                return string.Empty;
//            return "\n\nSources:\n"
//                + string.Join(
//                    "\n",
//                    hits.Select(h => $"[#${h.Ordinal}] {h.Source} (score={h.Score:0.000})")
//                );
//        }

//        private async Task HandleToolCall(
//            ChatToolCall call,
//            ToolRagChatRequest req,
//            List<ChatMessage> messages,
//            CancellationToken ct,
//            List<(int Ordinal, string Source, double Score)> lastHits
//        )
//        {
//            switch (call.FunctionName)
//            {
//                case nameof(SearchDocsTool):
//                    string argJson = call.FunctionArguments.ToString();
//                    using (var doc = JsonDocument.Parse(argJson))
//                    {
//                        var root = doc.RootElement;
//                        string query = root.TryGetProperty("query", out var q)
//                            ? q.GetString() ?? ""
//                            : "";
//                        int? topK = root.TryGetProperty("topK", out var tk)
//                            ? tk.GetInt32()
//                            : req.TopK;
//                        float? minScore = root.TryGetProperty("minScore", out var ms)
//                            ? (float?)ms.GetDouble()
//                            : req.MinScore;

//                        string resultJson = await _search.InvokeAsync(query, topK, minScore, ct);
//                        lastHits.Clear();
//                        lastHits.AddRange(ParseHits(resultJson));
//                        messages.Add(
//                            new ToolChatMessage(
//                                call.Id,
//                                [ChatMessageContentPart.CreateTextPart(resultJson)]
//                            )
//                        );
//                    }
//                    break;
//                default:
//                    messages.Add(
//                        new ToolChatMessage(
//                            call.Id,
//                            [
//                                ChatMessageContentPart.CreateTextPart(
//                                    $"Unknown tool: {call.FunctionName}"
//                                ),
//                            ]
//                        )
//                    );
//                    break;
//            }
//        }
//    }
//}
