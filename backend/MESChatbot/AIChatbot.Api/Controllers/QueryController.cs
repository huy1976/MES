using AIChatbot.Api.Models;
using AIChatbot.Api.Services;
using AIChatbot.Api.Services.Database;
using AIChatbot.Api.Services.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace AIChatbot.Api.Controllers
{
    [Authorize]
    [Route("query")]
    [ApiController]
    public class QueryController : ControllerBase
    {
        private readonly RagPipeline _rag;
        private readonly IDistributedCache _cache;
        private readonly ISqlDatabaseService _sqlDbService;

        public QueryController(
            RagPipeline rag,
            IDistributedCache cache,
            ISqlDatabaseService sqlDbService)
        {
            _rag = rag;
            _cache = cache;
            _sqlDbService = sqlDbService;
        }

        [HttpPost]
        public async Task<ActionResult<QueryResponse>> Ask(
            [FromBody] QueryRequest req,
            CancellationToken ct)
        {
            // Kiểm tra request có đi vào controller hay không
            Console.WriteLine("Đã vào QueryController");

            // Kiểm tra frontend có gửi JWT hay không
            var authHeader = Request.Headers.Authorization.ToString();
            Console.WriteLine($"Authorization Header: {authHeader}");

            // Kiểm tra câu hỏi có bị rỗng không
            if (string.IsNullOrWhiteSpace(req.Question))
            {
                return BadRequest("Question is required.");
            }

            // Lấy tên tài khoản từ JWT
            var userName =
                User.FindFirst(JwtRegisteredClaimNames.Name)?.Value
                ?? User.Identity?.Name;

            // Nếu không lấy được tên tài khoản thì từ chối request
            if (string.IsNullOrWhiteSpace(userName))
            {
                return Unauthorized(
                    "Không xác định được tài khoản đang đăng nhập.");
            }

            // Tạo Redis key riêng cho từng tài khoản
            // Ví dụ:
            // ChatHistory:admin
            // ChatHistory:admin1
            string historyKey = $"ChatHistory:{userName}";

            List<ChatMessage> chatHistory = new List<ChatMessage>();

            // Đọc lịch sử chat riêng của tài khoản từ Redis
            var chatHistoryJson =
                await _cache.GetStringAsync(historyKey, ct);

            if (!string.IsNullOrEmpty(chatHistoryJson))
            {
                chatHistory =
                    JsonSerializer.Deserialize<List<ChatMessage>>(
                        chatHistoryJson)
                    ?? new List<ChatMessage>();
            }

            try
            {
                // Gọi AI
                var response = await _rag.AskAsync(
                    req.Question,
                    chatHistory,
                    req.TopK,
                    ct);

                string finalAnswer = response.Answer;

                // =========================================================
                // LOGIC ĐÁNH CHẶN: EXCEL / CHART
                // =========================================================

                // Xóa thẻ Markdown và sửa trường hợp AI sinh 2 dấu nháy kép
                string cleanAnswer = finalAnswer
                    .Replace("```json", "")
                    .Replace("```", "")
                    .Replace("\"\"", "\"")
                    .Trim();

                if (cleanAnswer.StartsWith("{")
                    && cleanAnswer.Contains("Action"))
                {
                    try
                    {
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };

                        var actionData =
                            JsonSerializer.Deserialize<AiActionRequest>(
                                cleanAnswer,
                                options);

                        if (actionData != null)
                        {
                            if (actionData.Action == "ExportLastData")
                            {
                                string excelLink =
                                    await _sqlDbService
                                        .ExportLastDataToExcelAsync();

                                finalAnswer =
                                    $"{actionData.Message}\n\n{excelLink}";
                            }
                            else if (
                                actionData.Action == "NewQueryAndExport"
                                && !string.IsNullOrWhiteSpace(
                                    actionData.Query))
                            {
                                await _sqlDbService.ExecuteQueryAsync(
                                    actionData.Query);

                                string excelLink =
                                    await _sqlDbService
                                        .ExportLastDataToExcelAsync();

                                finalAnswer =
                                    $"{actionData.Message}\n\n{excelLink}";
                            }
                            else if (actionData.Action == "Chart")
                            {
                                string chartDataJson =
                                    await _sqlDbService
                                        .ExportLastDataForChartAsync();

                                finalAnswer =
                                    $"{actionData.Message}\n\n" +
                                    $"[CHART_READY|{actionData.ChartType}]\n" +
                                    chartDataJson;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        finalAnswer =
                            $"[HỆ THỐNG - LỖI PARSE JSON]: {ex.Message}" +
                            $"\n\nChuỗi của AI sinh ra:\n{cleanAnswer}";
                    }
                }

                // Thêm câu hỏi và câu trả lời vào lịch sử
                chatHistory.Add(new ChatMessage
                {
                    Role = "user",
                    Content = req.Question
                });

                chatHistory.Add(new ChatMessage
                {
                    Role = "assistant",
                    Content = finalAnswer
                });

                // Chỉ giữ lại 10 tin nhắn gần nhất
                if (chatHistory.Count > 10)
                {
                    chatHistory = chatHistory
                        .Skip(chatHistory.Count - 10)
                        .ToList();
                }

                // Lịch sử hết hạn sau 1 giờ không sử dụng
                var cacheOptions =
                    new DistributedCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromHours(1));

                string serializedHistory =
                    JsonSerializer.Serialize(chatHistory);

                // Lưu lịch sử vào Redis theo từng tài khoản
                await _cache.SetStringAsync(
                    historyKey,
                    serializedHistory,
                    cacheOptions,
                    ct);

                return Ok(finalAnswer);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    $"Lỗi server: {ex.Message}");
            }
        }
    }
}