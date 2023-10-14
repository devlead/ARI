using System.Net.Http.Json;
using System.Runtime.CompilerServices;

namespace ARI.Services.ARM;

public record TokenService(
    IHttpClientFactory HttpClientFactory,
    AzureTokenService AzureTokenService,
    ILogger<TokenService> Logger
    )
{
    static Azure.Core.AccessToken? cachedAccessToken = default;
    static Azure.Core.AccessToken? cachedGraphAccessToken = default;

    private async Task<string> GetAzureToken(string tenantId)
    {
        if (cachedAccessToken.HasValue && (cachedAccessToken.Value.ExpiresOn - DateTimeOffset.UtcNow).TotalMinutes > 1)
        {
            return cachedAccessToken.Value.Token;
        }

        Logger.LogInformation("Getting azure token...");
        try
        {
            var accessToken = await AzureTokenService(tenantId);
            cachedAccessToken = accessToken;
            return accessToken.Token;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to fetch Azure Access Token");
            throw;
        }
    }

    private async Task<string> GetGraphToken(string tenantId)
    {
        if (cachedGraphAccessToken.HasValue && (cachedGraphAccessToken.Value.ExpiresOn - DateTimeOffset.UtcNow).TotalMinutes > 1)
        {
            return cachedGraphAccessToken.Value.Token;
        }

        Logger.LogInformation("Getting Graph token...");
        try
        {
            var accessToken = await AzureTokenService(tenantId, "https://graph.microsoft.com/.default");
            cachedGraphAccessToken = accessToken;
            return accessToken.Token;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to fetch Graph Access Token");
            throw;
        }
    }

    public async Task<T> ARMHttpClientGetAsync<T>(
        string tenantId,
        string url,
        string? accept = "application/json"
        )
    {
        using var repoHttpClient = GetBearerTokenHttpClient(
            await GetAzureToken(tenantId),
            accept,
            null
            );

        return await GetFromJsonAsync<T>(repoHttpClient, url);
    }
    public async Task<T> GraphHttpClientGetAsync<T>(
       string tenantId,
       string url,
       string? accept = "application/json"
       )
    {
        using var repoHttpClient = GetBearerTokenHttpClient(
            await GetGraphToken(tenantId),
            accept,
            null
            );

        return await GetFromJsonAsync<T>(repoHttpClient, url);
    }

    private static async Task<T> GetFromJsonAsync<T>(HttpClient httpClient, string url)
    {
        var result = await httpClient.GetFromJsonAsync<T>(url);

        ArgumentNullException.ThrowIfNull(result);

        return result;
    }

    private HttpClient GetBearerTokenHttpClient(
        string bearerToken,
        string? accept = "application/json",
        string? contentType = "application/json",
        [CallerMemberName]
        string name = nameof(GetBearerTokenHttpClient)
        )
    {
        var bearerHttpClient = HttpClientFactory.CreateClient(name);

        bearerHttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
           "Bearer",
           bearerToken
        );

        if (!string.IsNullOrWhiteSpace(accept))
        {
            bearerHttpClient.DefaultRequestHeaders.Accept.TryParseAdd(accept);
        }

        if (!string.IsNullOrWhiteSpace(contentType))
        {
            bearerHttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", contentType);
        }

        return bearerHttpClient;
    }
}
