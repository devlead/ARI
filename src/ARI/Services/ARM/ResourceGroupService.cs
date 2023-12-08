namespace ARI.Services.ARM;

public record ResourceGroupService(
    ILogger<ResourceGroupService> Logger,
    TokenService TokenService
    )
{
    public async Task<ICollection<ResourceGroup>> GetResourceGroups(string tenantId, string subscriptionId)
    {
        var resourceGroups = await TokenService.ARMHttpClientGetAsync<ArmResult<ResourceGroup>>(
            tenantId,
            $"https://management.azure.com/subscriptions/{subscriptionId}/resourcegroups?api-version=2021-04-01"
            );

        ArgumentNullException.ThrowIfNull(resourceGroups.Value);

        return resourceGroups
                .Value
                .Index(
                    _ => tenantId,
                    _ => tenantId,
                    resourceGroup => resourceGroup.Name
                    );
    }
}
