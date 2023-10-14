namespace ARI.Models.Tenant;

public record AzureTenant(
    [property: JsonPropertyName("id")]
    string Id,
    [property: JsonPropertyName("tenantId")]
    string TenantId,
    // Only available for users
    [property: JsonPropertyName("countryCode")]
    string CountryCode,
    // Only available for users
    [property: JsonPropertyName("displayName")]
    string DisplayName,
    // Only available for users
    [property: JsonPropertyName("domains")]
    string[] Domains,
    // Only available for users
    [property: JsonPropertyName("tenantCategory")]
    string TenantCategory,
    // Only available for users
    [property: JsonPropertyName("defaultDomain")]
    string DefaultDomain,
    // Only available for users
    [property: JsonPropertyName("tenantType")]
    string TenantType,
    // Only available for users
    [property: JsonPropertyName("tenantBrandingLogoUrl")]
    string TenantBrandingLogoUrl
) :AzureResourceBase
{
    public override string PublicId => DefaultDomain;
    public override string Description => DisplayName;
}
