namespace ARI.Extensions;

public static class AzureResourceExtensions
{
    public static ICollection<TSource> Index<TSource>(
        this IEnumerable<TSource> values,
        Func<TSource, string>? tenantIdFunc = default,
        Func<TSource, string>? subscriptionIdFunc = default,
        Func<TSource, string>? resourceGroupNameFunc = default,
        Func<TSource, bool>? predicate = default
        )
        where TSource : AzureResourceBase
    {
        var order = 0;
        return values
            .Where(value => predicate?.Invoke(value) ?? true)
            .OrderBy(resource => resource.Description, StringComparer.OrdinalIgnoreCase)
            .Select(resource => resource.Index(
                tenantIdFunc,
                subscriptionIdFunc,
                resourceGroupNameFunc,
                ++order
                ))
            .ToArray();
    }

    public static TSource Index<TSource>(
        this TSource resource,
        Func<TSource, string>? tenantIdFunc = default,
        Func<TSource, string>? subscriptionIdFunc = default,
        Func<TSource, string>? resourceGroupNameFunc = default,
        int order = 1
        )
        where TSource : AzureResourceBase
    {
        return resource with
        {
            TenantId = tenantIdFunc?.Invoke(resource) ?? resource.TenantId,
            SubscriptionId = subscriptionIdFunc?.Invoke(resource) ?? resource.SubscriptionId,
            ResourceGroupName = resourceGroupNameFunc?.Invoke(resource) ?? resource.ResourceGroupName,
            Order = order
        };
    }
}