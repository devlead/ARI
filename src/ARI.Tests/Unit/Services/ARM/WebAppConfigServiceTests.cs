namespace ARI.Tests.Unit.Services.ARM;

[TestFixture]
public class WebAppConfigServiceTests
{
    [TestCase(Constants.Tenant.Subscription1.Id, Constants.Tenant.Subscription1.ResourceGroup1.Name, Constants.Tenant.Subscription1.ResourceGroup1.Site.Name)]
    [TestCase(Constants.Tenant.Subscription1.Id, Constants.Tenant.Subscription1.ResourceGroup2.Name, Constants.Tenant.Subscription1.ResourceGroup2.Site.Name)]
    [TestCase(Constants.Tenant.Subscription1.Id, Constants.Tenant.Subscription1.ResourceGroup3.Name, Constants.Tenant.Subscription1.ResourceGroup3.Site.Name)]
    public async Task GetWebAppConfig(string subscriptionId, string resourceGroupName, string siteName)
    {
        // Given
        var webAppConfigService = ARIServiceProviderFixture
                                    .GetRequiredService<WebAppConfigService>();

        // When
        var result = await webAppConfigService.GetWebAppConfig(Constants.Tenant.Id, subscriptionId, resourceGroupName, siteName);

        // Then
        await Verify(result);
    }
}
