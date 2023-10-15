using Newtonsoft.Json.Linq;

namespace ARI.Extensions;

public static class StringMarkdownExtensions
{
    public static string CodeLine(this string? value)
        => string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : $"`{value.SingleLine()}`";

    public static string PreLine(this string? value)
      => string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : $"<pre style=\"white-space: pre-wrap;\">{value.SingleLine()}</pre>";

    public static string? SingleLine(this string? value)
        => value?.NormalizeLineEndings().ReplaceLineEndings("<br>");

    public static string Bold(this string? value)
        => string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : $"**{value.SingleLine()}**";

    public static string Link(this string description, string? href = default)
        => string.IsNullOrWhiteSpace(description)
            ? string.Empty
#pragma warning disable SYSLIB0013 // Type or member is obsolete
            : $"[{description}]({Uri.EscapeUriString(href ?? description)})";
#pragma warning restore SYSLIB0013 // Type or member is obsolete

    public static string LastPart(this string? value, char separator)
        => value?.Split(separator, StringSplitOptions.TrimEntries) 
            is string[] parts
                ? parts[^1]
                : string.Empty;
}