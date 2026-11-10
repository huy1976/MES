namespace AIChatbot.Api.Services.Database;

public interface ISqlDatabaseService
{
    Task<string> GetDatabaseSystemPromptAsync();
    Task<string> ExecuteQueryAsync(string sqlQuery);
    Task<string> ExportLastDataToExcelAsync();
    Task<string> ExportLastDataForChartAsync();
}
