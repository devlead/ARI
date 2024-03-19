using Microsoft.Extensions.Primitives;
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
            : $"[{description}]({PathEscapeUriString(href ?? description)})";

    public static string PathEscapeUriString(this string path)
        => path.Aggregate(
                new StringBuilder(),
                (sb, c) => 
                            !char.IsAsciiLetterOrDigit(c)
                            && c != '/'
                            && c!='-'
                            && c != '_'
                            && c != '.' 
                                ? sb.Append("0x").Append(Convert.ToHexString(Encoding.UTF8.GetBytes(new[] { c })))
                                : sb.Append(c),
                sb => sb.ToString()
            );
#pragma warning restore SYSLIB0013 // Type or member is obsolete

    public static string LastPart(this string? value, char separator)
        => value?.Split(separator, StringSplitOptions.TrimEntries) 
            is string[] parts
                ? parts[^1]
                : string.Empty;

    public static string SeparateByCase(this string? value, char separator = ' ')
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }


        var sb = new StringBuilder(value[..1].ToUpperInvariant());
        
        for (var i = 1; i < value.Length; i++)
        {
            var c = value[i];
            var c2 = value[i-1];
            if (char.IsUpper(c) && !(c2=='I' && c=='D'))
            {
                sb.Append(separator);
            }

            sb.Append(c);
        }

        return sb.ToString();
    }
}