namespace ARI.Tests.Unit.Services.ARM;

[TestFixture]
public class WebAppSettingsServiceTests
{
    [TestCase(Constants.Tenant.Subscription1.Id, Constants.Tenant.Subscription1.ResourceGroup1.Name, Constants.Tenant.Subscription1.ResourceGroup1.Site.Name)]
    [TestCase(Constants.Tenant.Subscription1.Id, Constants.Tenant.Subscription1.ResourceGroup2.Name, Constants.Tenant.Subscription1.ResourceGroup2.Site.Name)]
    [TestCase(Constants.Tenant.Subscription1.Id, Constants.Tenant.Subscription1.ResourceGroup3.Name, Constants.Tenant.Subscription1.ResourceGroup3.Site.Name)]
    public async Task GetWebAppSettings(string subscriptionId, string resourceGroupName, string siteName)
    {
        // Given
        var webAppSettingsService = ARIServiceProviderFixture
                                    .GetRequiredService<WebAppSettingsService>();

        // When
        var result = await webAppSettingsService.GetWebAppSettings(Constants.Tenant.Id, subscriptionId, resourceGroupName, siteName);

        // Then
        await Verify(result);
    }
}