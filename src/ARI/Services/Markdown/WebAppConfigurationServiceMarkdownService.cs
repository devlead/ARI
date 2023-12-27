namespace ARI.Services.Markdown;

public record WebAppConfigurationServiceMarkdownService(
    WebAppConfigService WebAppConfigurationService,
    ILogger<WebAppConfigurationServiceMarkdownService> Logger
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
