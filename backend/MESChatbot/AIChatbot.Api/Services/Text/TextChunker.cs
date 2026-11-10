using System.Text;

namespace AIChatbot.Api.Services.Text;
public static class TextChunker
{
    public static IEnumerable<string> Chunk(string text, int chunkSize, int overlap)
    {
        if (string.IsNullOrWhiteSpace(text)) yield break;
        var cleaned = Normalize(text);
        int start = 0;
        while (start < cleaned.Length)
        {
            int end = Math.Min(start + chunkSize, cleaned.Length);
            // cắt đến ranh giới câu nếu có
            int lastPeriod = cleaned.LastIndexOfAny(new[] { '.', '!', '?', '\n' }, end - 1, Math.Min(100, end - start));
            if (lastPeriod > start + 100) end = lastPeriod + 1;
            yield return cleaned.Substring(start, end - start).Trim();
            if (end == cleaned.Length) break;
            start = Math.Max(0, end - overlap);
        }
    }
    static string Normalize(string s)
    {
        var sb = new StringBuilder(s.Length);
        foreach (var ch in s)
            sb.Append(char.IsControl(ch) && ch != '\n' && ch != '\r' ? ' ' : ch);
        return sb.ToString().Replace("\r\n", "\n").Replace("\r", "\n");
    }
}
