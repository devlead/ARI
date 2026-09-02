namespace ARI.Tests.Unit.Services.ARM;

[TestFixture]
public class ResourceGroupServiceTests
{
    [TestCase(Constants.Tenant.Subscription1.Id)]
    [TestCase(Constants.Tenant.Subscription2.Id)]
    public async Task GetSubscriptions(string subscriptionId)
    {
        // Given
        var resourceGroupService = ServiceProviderFixture
                                    .GetRequiredService<ResourceGroupService>();

        // When
        var result = await resourceGroupService.GetResourceGroups(Constants.Tenant.Id, subscriptionId);

        // Then
        await Verify(result);
    }
}
