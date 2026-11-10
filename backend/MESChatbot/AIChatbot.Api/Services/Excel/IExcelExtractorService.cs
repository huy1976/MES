namespace AIChatbot.Api.Services.Excel;

public interface IExcelExtractorService
{
    public List<(string text, string source)> ExtractChunks(Stream excelStream, string fileName);
}
