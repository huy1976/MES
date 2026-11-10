using AIChatbot.Api.Models;
using AIChatbot.Api.Services.Text;
using AIChatbot.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AIChatbot.Api.Services.Excel;

namespace AIChatbot.Api.Controllers;

[ApiController]
[Route("ingest")]
public class IngestController : ControllerBase
{
    private readonly RagPipeline _rag;
    private readonly AppConfig _cfg;
    private readonly IExcelExtractorService _excelExtractor;
    public IngestController(RagPipeline rag, AppConfig cfg, IExcelExtractorService excelExtractor)
    {
        _rag = rag;
        _cfg = cfg;
        _excelExtractor = excelExtractor;
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [DisableRequestSizeLimit]
    [RequestFormLimits(MultipartBodyLengthLimit = 1024L * 1024L * 200L)] // 200MB
    public async Task<IActionResult> Upload([FromForm] List<IFormFile> files, CancellationToken ct)
    {
        if (files == null || files.Count == 0) return BadRequest("No files.");

        var chunks = new List<(string text, string? source)>();

        foreach (var file in files)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            // 2. Bổ sung ".xlsx" vào danh sách cho phép
            if (ext is not (".pdf" or ".txt" or ".md" or ".xlsx")) continue;

            // ==========================================
            // XỬ LÝ RIÊNG CHO FILE EXCEL (.xlsx)
            // ==========================================
            if (ext == ".xlsx")
            {
                // Đọc trực tiếp từ RAM, không cần tạo file tạm dưới ổ cứng
                using var stream = file.OpenReadStream();

                // Lấy ra danh sách các chunk đã được ghép "Tiêu đề cột: Giá trị"
                var excelChunks = _excelExtractor.ExtractChunks(stream, file.FileName);

                foreach (var chunk in excelChunks)
                {
                    chunks.Add((chunk.text, chunk.source));
                }

                continue; // Xong file Excel thì nhảy sang file tiếp theo
            }

            // ==========================================
            // XỬ LÝ CHO PDF, TXT, MD (Giữ nguyên logic của bạn)
            // ==========================================
            var tmp = Path.GetTempFileName();
            await using (var fs = System.IO.File.Create(tmp))
                await file.CopyToAsync(fs, ct);

            string raw = ext switch
            {
                ".pdf" => PdfTextExtractor.Extract(tmp),
                ".txt" or ".md" => await System.IO.File.ReadAllTextAsync(tmp, ct),
                _ => ""
            };
            System.IO.File.Delete(tmp);

            if (string.IsNullOrWhiteSpace(raw)) continue;

            foreach (var ch in TextChunker.Chunk(raw, _cfg.Rag.ChunkSize, _cfg.Rag.ChunkOverlap))
                chunks.Add((ch, file.FileName));
        }

        // 3. Cập nhật câu thông báo lỗi
        if (chunks.Count == 0) return BadRequest("No extractable text. Support: .pdf, .txt, .md, .xlsx");

        await _rag.IngestAsync(chunks, ct);

        return Ok(new { Ingested = chunks.Count, Files = files.Count });
    }
}
