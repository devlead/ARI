namespace ARI.Services.ARM;

public record WebAppConfigService(
    ILogger<WebAppConfigService> Logger,
    TokenService TokenService
    )
{
    public async Task<WebConfig> GetWebAppConfig(
        string tenantId,
        string subscriptionId,
        string resourceGroupName,
        string webAppName
        )
    {
        var config = await TokenService.ARMHttpClientGetAsync<ArmResult<WebConfig>>(
            tenantId,
            $"https://management.azure.com/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/providers/Microsoft.Web/sites/{webAppName}/config?api-version=2022-03-01"
            );

        ArgumentNullException.ThrowIfNull(config.Value);

        return config
                .Value
                .IndexResources(
                    _ => tenantId,
                    _ => subscriptionId,
                    _ => resourceGroupName
                )
                .Single();
    }
}
