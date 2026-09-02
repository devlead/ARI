using ARI.Tests;
using Azure.Core;
using Devlead.Testing.MockHttp;

public static partial class ServiceProviderFixture
{
    static partial void InitServiceProvider(IServiceCollection services)
    {
        services
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
            .AddSingleton<MarkdownServiceBase, WebAppConfigurationServiceMarkdownService>()
            .AddSingleton<WebAppSettingsService>()
            .AddSingleton<MarkdownServiceBase, WebAppSettingsServiceMarkdownService>()
            .AddMockHttpClient<Constants>();
    }
}
