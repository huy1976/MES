
using ClosedXML.Excel;
using System.Text;

namespace AIChatbot.Api.Services.Excel;

public class ExcelExtractorService : IExcelExtractorService
{
    public List<(string text, string source)> ExtractChunks(Stream excelStream, string fileName)
    {
            var chunks = new List<(string text, string source)>();

            using var workbook = new XLWorkbook(excelStream);

            foreach (var worksheet in workbook.Worksheets)
            {
                // Bỏ qua các sheet trống
                var rows = worksheet.RowsUsed();
                if (!rows.Any()) continue;

                var headers = new List<string>();
                bool isFirstRow = true;
                int maxColumns = 0;

                foreach (var row in rows)
                {
                    // 1. Dòng đầu tiên luôn được coi là Header (Tiêu đề cột)
                    if (isFirstRow)
                    {
                        // Quét lấy danh sách tiêu đề
                        var cells = row.CellsUsed();
                        maxColumns = cells.LastOrDefault()?.Address.ColumnNumber ?? 0;

                        for (int i = 1; i <= maxColumns; i++)
                        {
                            headers.Add(row.Cell(i).GetString().Trim());
                        }
                        isFirstRow = false;
                        continue;
                    }

                    // 2. Xử lý các dòng dữ liệu tiếp theo
                    var rowContext = new StringBuilder();
                    rowContext.AppendLine($"[Nguồn: File {fileName}, Sheet: {worksheet.Name}]");
                    rowContext.AppendLine("Thông tin bản ghi:");

                    bool hasData = false;

                    for (int i = 1; i <= headers.Count; i++)
                    {
                        var cellValue = row.Cell(i).GetString().Trim();
                        var headerName = string.IsNullOrWhiteSpace(headers[i - 1]) ? $"Cột {i}" : headers[i - 1];

                        if (!string.IsNullOrWhiteSpace(cellValue))
                        {
                            // Ghép Tiêu đề cột + Giá trị ô (VD: "Tên sản phẩm: Máy kéo dây")
                            rowContext.AppendLine($"- {headerName}: {cellValue}");
                            hasData = true;
                        }
                    }

                    // 3. Đưa nguyên khối text của dòng này vào mảng Chunks
                    if (hasData)
                    {
                        chunks.Add((rowContext.ToString(), fileName));
                    }
                }
            }

            return chunks;
    }
}
