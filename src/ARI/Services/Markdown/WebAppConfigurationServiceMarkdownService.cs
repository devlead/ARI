namespace ARI.Services.Markdown;

public abstract record WebAppServiceBase()
    : MarkdownServiceBase("Microsoft.Web/sites")
{
}

public record WebAppConfigServiceMarkdownService(
    WebAppConfigService WebAppConfigurationService,
    ILogger<WebAppConfigServiceMarkdownService> Logger
    ) : WebAppServiceBase
{
    public override async Task Render(
        TextWriter writer,
        Resource resource,
        InventorySettings settings
        )
    {
        var webAppConfig = await WebAppConfigurationService.GetWebAppConfig(
            resource.TenantId,
            resource.SubscriptionId,
            resource.ResourceGroupName,
            resource.Name
            );

        await writer.AddProperties(
            webAppConfig.Properties,
            settings
            );
    }
}
