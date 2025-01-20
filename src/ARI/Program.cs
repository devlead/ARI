using ARI.Commands;
using Azure.Core;
using Azure.Identity;

public partial class Program
{
    static partial void AddServices(IServiceCollection services)
    {
        services
            .AddCakeCore()
            .AddSingleton<AzureTokenService>(
                async (tenantId, scope) =>
                {
                    var tokenCredential = new DefaultAzureCredential();
                    var accessToken = await tokenCredential.GetTokenAsync(
                        new TokenRequestContext(
                            tenantId: tenantId,
                            scopes: [
                                scope ?? "https://management.azure.com/.default"
                            ]
                            )
                    );
                    return accessToken;
                }
            )
            .AddSingleton<InventoryCommand>()
            .AddSingleton<TokenService>()
            .AddSingleton<TenantService>()
            .AddSingleton<SubscriptionService>()
            .AddSingleton<ResourceGroupService>()
            .AddSingleton<ResourceService>()
            .AddSingleton<WebAppConfigService>()
            .AddSingleton<WebAppSettingsService>()
            .AddSingleton<MarkdownServiceBase, WebAppConfigurationServiceMarkdownService>()
            .AddSingleton<MarkdownServiceBase, WebAppSettingsServiceMarkdownService>();

        services.AddHttpClient();
    }

    static partial void ConfigureApp(AppServiceConfig appServiceConfig)
    {
        appServiceConfig.SetApplicationName("ari");

        appServiceConfig.AddCommand<InventoryCommand>("inventory")
                .WithDescription("Example inventory command.")
                .WithExample(["inventory", "00000000-0000-0000-0000-000000000000", "outputpath"]);
    }
}