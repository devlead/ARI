namespace ARI.Models.Graph;

public record GraphOrg(
    [property: JsonPropertyName("displayName")]
    string DisplayName,
    [property: JsonPropertyName("countryLetterCode")]
    string CountryLetterCode,
    [property: JsonPropertyName("tenantType")]
    string TenantType,
    [property: JsonPropertyName("verifiedDomains")]
    GraphOrgDomain[] VerifiedDomains
    );
