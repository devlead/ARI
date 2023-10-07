namespace ARI.Services.ARM;

public record TenantService(
    ILogger<TenantService> Logger,
    TokenService TokenService
    )
{
    public async Task<AzureTenant> GetTenant(string tenantId)
    {
        var tenants = await TokenService.ARMHttpClientGetAsync<ArmResult<AzureTenant>>(
            tenantId,
            "https://management.azure.com/tenants?api-version=2020-01-01"
            );

        ArgumentNullException.ThrowIfNull(tenants?.Value);

        var tenant = tenants.Value.Single(tenant => tenant.TenantId == tenantId);

        if (string.IsNullOrEmpty(tenant.DisplayName))
        {
            var org = await TokenService.GraphHttpClientGetAsync<GraphOrg>(
                tenantId,
                $"https://graph.microsoft.com/beta/organization/{tenantId}"
                );
            return tenant with
            {
                DisplayName = org.DisplayName,
                CountryCode = org.CountryLetterCode,
                TenantType = org.TenantType,
                DefaultDomain = org.VerifiedDomains.FirstOrDefault(domain => domain.IsDefault)?.Name ?? string.Empty,
                Domains = org.VerifiedDomains.Select(domain => domain.Name).ToArray()
            };

        }
        return tenant;
    }
}
