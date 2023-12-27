namespace ARI.Services.Markdown;

public record WebAppSettingsServiceMarkdownService(
    WebAppSettingsService WebAppSettingsService,
    ILogger<WebAppSettingsServiceMarkdownService> Logger
    ) : WebAppServiceBase
{
    public override async Task Render(
        TextWriter writer,
        Resource resource,
        InventorySettings settings
        )
    {
        if (!settings.IncludeSiteApplicationsettings)
        {
            return;
        }

        var webAppSettings = await WebAppSettingsService.GetWebAppSettings(
            resource.TenantId,
            resource.SubscriptionId,
            resource.ResourceGroupName,
            resource.Name
            );

        await writer.AddSettings(
            webAppSettings.Properties,
            settings
            );
    }
}