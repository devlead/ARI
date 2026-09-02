namespace ARI.Tests.Unit.Services.ARM;

[TestFixture]
public class TenantServiceTests
{
    [Test]
    public async Task GetTenant()
    {
        // Given
        var tenantService = ServiceProviderFixture
                                .GetRequiredService<TenantService>();

        // When
        var result = await tenantService.GetTenant(Constants.Tenant.Id);

        // Then
        await Verify(result);
    }
}
