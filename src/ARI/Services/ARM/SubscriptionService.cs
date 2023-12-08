namespace ARI.Services.ARM;

public record SubscriptionService(
    ILogger<SubscriptionService> Logger,
    TokenService TokenService
    )
{
    public async Task<ICollection<Subscription>> GetSubscriptions(string tenantId)
    {
        var subscriptions = await TokenService.ARMHttpClientGetAsync<ArmResult<Subscription>>(
            tenantId,
            "https://management.azure.com/subscriptions?api-version=2020-01-01"
            );

        ArgumentNullException.ThrowIfNull(subscriptions.Value);
 
        return subscriptions
                .Value
                .Index()
                .ToArray();
    }
}
