using System.Text.Json.Nodes;

namespace ARI.Services.ARM;

public record ResourceService(
    ILogger<ResourceService> Logger,
    TokenService TokenService
    )
{
    public async Task<ICollection<Resource>> GetResources(
        string tenantId,
        string subscriptionId,
        string resourceGroupName
        )
    {
        var resources = await TokenService.ARMHttpClientGetAsync<ArmResult<Resource>>(
            tenantId,
            $"https://management.azure.com/subscriptions/{subscriptionId}/resourceGroups/{resourceGroupName}/resources?$expand=createdTime,changedTime,provisioningState&api-version=2021-04-01"
            );

        ArgumentNullException.ThrowIfNull(resources.Value);

        return resources
                .Value
                .Index(
                    _ => tenantId,
                    _ => subscriptionId,
                    _ => resourceGroupName
                 );
    }
}
