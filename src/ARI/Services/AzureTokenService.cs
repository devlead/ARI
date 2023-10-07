namespace ARI.Services;

public delegate Task<Azure.Core.AccessToken> AzureTokenService(string tenantId, string? scope = null);