using ARI.Models.Tenant.Subscription;
using ARI.Models;
using ARI.Models.Tenant;
using ARI.Models.Graph;

namespace ARI.Tests.Unit.Services.ARM;

[TestFixture]
public class TokenServiceTests
{
    [TestCase<ArmResult<AzureTenant>>(Constants.Request.Uri.Tenants)]
    [TestCase<ArmResult<Subscription>>(Constants.Request.Uri.Subscriptions)]
    public async Task ARMHttpClientGetAsync<T>(string url)
    {
        // Given
        var tokenService = ARIServiceProviderFixture
                                .GetRequiredService<TokenService>();

        // When
        var result = await tokenService.ARMHttpClientGetAsync<T>(
            Constants.Tenant.Id,
            url
            );

        // Then
        await Verify(result);
    }

    [TestCase<GraphOrg>(Constants.Request.Uri.GraphOrg)]
    public async Task GraphHttpClientGetAsync<T>(string url)
    {
        // Given
        var tokenService = ARIServiceProviderFixture
                                .GetRequiredService<TokenService>();

        // When
        var result = await tokenService.GraphHttpClientGetAsync<T>(
            Constants.Tenant.Id,
            url
            );

        // Then
        await Verify(result);
    }
}


