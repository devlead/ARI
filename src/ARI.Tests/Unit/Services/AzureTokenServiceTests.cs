namespace ARI.Tests.Unit.Services;

[TestFixture]
public class AzureTokenServiceTests
{
    [Test]
    public async Task GetToken()
    {
        // Given
        var azureTokenService = ARIServiceProviderFixture
                                .GetRequiredService<AzureTokenService>();

        // When
        var result = await azureTokenService(Constants.Tenant.Id);

        // Then
        await Verify(result);
    }
}
