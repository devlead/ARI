namespace ARI.Extensions;

public static class AzureResourceExtensions
{
    public static ICollection<TSource> Index<TSource>(
        this IEnumerable<TSource> values,
        Func<TSource, bool>? predicate = default
        )
        where TSource : AzureResourceBase
    {
        var order = 0;
        return values
            .Where(value => predicate?.Invoke(value) ?? true)
            .OrderBy(resourceGroup => resourceGroup.Description, StringComparer.OrdinalIgnoreCase)
            .Select(resourceGroup => resourceGroup with { Order = ++order })
            .ToArray();
    }
}