namespace ARI.Tests.Unit.Services.ARM;

[TestFixture]
public class SubscriptionServiceTests
{
    [Test]
    public async Task GetSubscriptions()
    {
        // Given
        var subscriptionService = ServiceProviderFixture
                                    .GetRequiredService<SubscriptionService>();

        // When
        var result = await subscriptionService.GetSubscriptions(Constants.Tenant.Id);

        // Then
        await Verify(result);
    }
}
