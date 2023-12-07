using Azure.Core;

namespace ARI.Tests.Fixture;

public static class ARIServiceProviderFixture
{
    public static (T1, T2, T3, T4, T5, T6, T7) GetRequiredService<T1, T2, T3, T4, T5, T6, T7>(
       Func<IServiceCollection, IServiceCollection>? configure = null
       ) where T1 : notnull
            where T2 : notnull
            where T3 : notnull
            where T4 : notnull
            where T5 : notnull
            where T6 : notnull
            where T7 : notnull
    {
        var provider = GetServiceProvider(configure);
        return (
            provider.GetRequiredService<T1>(),
            provider.GetRequiredService<T2>(),
            provider.GetRequiredService<T3>(),
            provider.GetRequiredService<T4>(),
            provider.GetRequiredService<T5>(),
            provider.GetRequiredService<T6>(),
            provider.GetRequiredService<T7>()
            );
    }
    public static (T1, T2, T3, T4, T5, T6) GetRequiredService<T1, T2, T3, T4, T5, T6>(
       Func<IServiceCollection, IServiceCollection>? configure = null
       ) where T1 : notnull
            where T2 : notnull
            where T3 : notnull
            where T4 : notnull
            where T5 : notnull
            where T6 : notnull
    {
        var provider = GetServiceProvider(configure);
        return (
            provider.GetRequiredService<T1>(),
            provider.GetRequiredService<T2>(),
            provider.GetRequiredService<T3>(),
            provider.GetRequiredService<T4>(),
            provider.GetRequiredService<T5>(),
            provider.GetRequiredService<T6>()
            );
    }

    public static (T1, T2, T3, T4, T5) GetRequiredService<T1, T2, T3, T4, T5>(
       Func<IServiceCollection, IServiceCollection>? configure = null
       )    where T1 : notnull
            where T2 : notnull
            where T3 : notnull
            where T4 : notnull
            where T5 : notnull
    {
        var provider = GetServiceProvider(configure);
        return (
            provider.GetRequiredService<T1>(),
            provider.GetRequiredService<T2>(),
            provider.GetRequiredService<T3>(),
            provider.GetRequiredService<T4>(),
            provider.GetRequiredService<T5>()
            );
    }



    public static (T1, T2, T3, T4) GetRequiredService<T1, T2, T3, T4>(
       Func<IServiceCollection, IServiceCollection>? configure = null
       )    where T1 : notnull
            where T2 : notnull
            where T3 : notnull
            where T4 : notnull
    {
        var provider = GetServiceProvider(configure);
        return (
            provider.GetRequiredService<T1>(),
            provider.GetRequiredService<T2>(),
            provider.GetRequiredService<T3>(),
            provider.GetRequiredService<T4>()
            );
    }

    public static (T1, T2, T3) GetRequiredService<T1, T2, T3>(
        Func<IServiceCollection, IServiceCollection>? configure = null
        )   where T1 : notnull
            where T2 : notnull
            where T3 : notnull
    {
        var provider = GetServiceProvider(configure);
        return (
            provider.GetRequiredService<T1>(),
            provider.GetRequiredService<T2>(),
            provider.GetRequiredService<T3>()
            );
    }

    public static (T1, T2) GetRequiredService<T1, T2>(
        Func<IServiceCollection, IServiceCollection>? configure = null
        )   where T1 : notnull
            where T2 : notnull
    {
        var provider = GetServiceProvider(configure);
        return (
            provider.GetRequiredService<T1>(),
            provider.GetRequiredService<T2>()
            );
    }

    public static T GetRequiredService<T>(
        Func<IServiceCollection, IServiceCollection>? configure = null
        ) where T : notnull
        => GetServiceProvider(configure)
            .GetRequiredService<T>();

    public static ServiceProvider GetServiceProvider(Func<IServiceCollection, IServiceCollection>? configure)
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection
            .AddLogging()
            .AddSingleton<AzureTokenService>(
                (tenantId, scope) => Task.FromResult(new AccessToken(nameof(AccessToken), DateTimeOffset.UtcNow.AddDays(1)))
            )
            .AddSingleton<TokenService>()
            .AddSingleton<TenantService>()
            .AddSingleton<SubscriptionService>()
            .AddSingleton<ResourceGroupService>()
            .AddSingleton<ResourceService>()
            .AddSingleton<WebAppConfigService>()
            .AddSingleton<MarkdownServiceBase, WebAppConfigServiceMarkdownService>()
            .AddMockHttpClient();

            return (configure?.Invoke(serviceCollection) ?? serviceCollection).BuildServiceProvider();
        }
}
