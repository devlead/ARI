namespace ARI.Tests.Unit.Services.ARM;

[TestFixture]
public class ResourceServiceTests
{
    [TestCase(Constants.Tenant.Subscription1.Id, Constants.Tenant.Subscription1.ResourceGroup1.Name)]
    [TestCase(Constants.Tenant.Subscription1.Id, Constants.Tenant.Subscription1.ResourceGroup2.Name)]
    [TestCase(Constants.Tenant.Subscription1.Id, Constants.Tenant.Subscription1.ResourceGroup3.Name)]
    [TestCase(Constants.Tenant.Subscription2.Id, Constants.Tenant.Subscription2.ResourceGroup1.Name)]
    public async Task GetSubscriptions(string subscriptionId, string resourceGroupName)
    {
        // Given
        var resourceService = ARIServiceProviderFixture
                                    .GetRequiredService<ResourceService>();

        // When
        var result = await resourceService.GetResources(Constants.Tenant.Id, subscriptionId, resourceGroupName);

        // Then
        await Verify(result);
    }
}
