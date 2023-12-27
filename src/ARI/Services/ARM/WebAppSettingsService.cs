namespace ARI.Services.ARM;

public record WebAppSettingsService(
    ILogger<WebAppSettingsService> Logger,
    TokenService TokenService
    )
{
    public async Task<WebSettings> GetWebAppSettings(
        string tenantId,
        string subscriptionId,
        string resourceGroupName,
        string webAppName
        )
    {
        var settings = await TokenService.ARMHttpClientPostAsync<object,WebSettings>(
            tenantId,
            $"https://management.azure.com/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Web/sites/{webAppName}/config/appsettings/list?api-version=2022-03-01"
            );

        ArgumentNullException.ThrowIfNull(settings.Properties);

        return settings
                .Index(
                    _ => tenantId,
                    _ => subscriptionId,
                    _ => resourceGroupName
                );
    }
}