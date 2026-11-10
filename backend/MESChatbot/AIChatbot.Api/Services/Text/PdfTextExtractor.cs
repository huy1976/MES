using System.Text;
using UglyToad.PdfPig;

namespace AIChatbot.Api.Services.Text;

public static class PdfTextExtractor
{
    public static string Extract(string filePath)
    {
        var sb = new StringBuilder();
        using var doc = PdfDocument.Open(filePath);
        foreach (var page in doc.GetPages())
            sb.AppendLine(page.Text);
        return sb.ToString();
    }
}
