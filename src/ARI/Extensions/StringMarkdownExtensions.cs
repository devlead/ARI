namespace ARI.Extensions;

public static class StringMarkdownExtensions
{
    public static string CodeLine(this string? value)
        => $"`{value.SingleLine()}`";

    public static string PreLine(this string? value)
      => $"<pre style=\"white-space: pre-wrap;\">{value.SingleLine()}</pre>";

    public static string? SingleLine(this string? value)
        => value?.NormalizeLineEndings().ReplaceLineEndings("<br>");
}