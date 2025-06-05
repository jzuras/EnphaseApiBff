using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using EnphaseApiBff.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace EnphaseApiBff.Server;

public class EnphaseBffService : IEnphaseBffService
{
    private string EnphaseApiBaseUrl { get; set; } = "https://api.enphaseenergy.com/api/v4/systems";

    private HttpClient HttpClient { get; set; }
    private IHttpContextAccessor HttpContextAccessor { get; set; }
    private IConfiguration Configuration { get; set; }
    private string? CachedAccessToken { get; set; } // Used only if unable to sign in (see GetAccessTokenAsync / RefreshAccessTokenAsync).

    public EnphaseBffService(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration)
    {
        this.HttpClient = httpClient;
        this.HttpContextAccessor = httpContextAccessor;
        this.Configuration = configuration;
    }

    private async Task<string?> SetupEnphaseHttpClientAsync(int systemId)
    {
        if (systemId == 0)
        {
            throw new InvalidOperationException("System ID not found.");
        }

        var accessToken = await this.GetAccessTokenAsync();
        if (string.IsNullOrEmpty(accessToken) is true)
        {
            throw new InvalidOperationException("Access token not found.");
        }

        var apiKey = this.Configuration["Enphase:ApiKey"];
        if (string.IsNullOrEmpty(apiKey) is true)
        {
            throw new InvalidOperationException("Enphase API key not configured.");
        }

        this.HttpClient.DefaultRequestHeaders.Clear();
        this.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        return apiKey;
    }

    private async Task<string?> GetAccessTokenAsync()
    {
        var context = this.HttpContextAccessor.HttpContext;
        if (context?.User?.Identity?.IsAuthenticated is not true)
        {
            throw new InvalidOperationException("User is not authenticated.");
        }

        // Check if token is expired or about to expire (within 5 minutes).
        var expiresAtString = await context.GetTokenAsync("expires_at");
        if (string.IsNullOrEmpty(expiresAtString) is false && DateTimeOffset.TryParse(expiresAtString, out var expiresAt) is true)
        {
            if (expiresAt <= DateTimeOffset.UtcNow.AddMinutes(5))
            {
                this.CachedAccessToken = null;
                var refreshed = await RefreshAccessTokenAsync();
                if (refreshed is false)
                {
                    if(this.CachedAccessToken is not null)
                    {
                        Console.WriteLine("Using cached access token.");
                        return this.CachedAccessToken;
                    }

                    throw new InvalidOperationException("Access token expired and refresh failed.");
                }
            }
        }

        return await context.GetTokenAsync("access_token");
    }

    private async Task<bool> RefreshAccessTokenAsync()
    {
        var context = this.HttpContextAccessor.HttpContext;
        if (context is null)
        {
            Console.WriteLine("No HTTP context available.");
            return false;
        }

        try
        {
            // Get refresh token.
            var refreshToken = await context.GetTokenAsync("refresh_token");
            if (string.IsNullOrEmpty(refreshToken) is true)
            {
                Console.WriteLine("No refresh token available.");
                return false;
            }

            // Create refresh request.
            using var client = new HttpClient();
            var refreshRequest = new HttpRequestMessage(HttpMethod.Post,
                $"https://api.enphaseenergy.com/oauth/token?grant_type=refresh_token&refresh_token={refreshToken}");

            // Add basic auth header.
            var credentials = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{this.Configuration["Enphase:ClientId"]}:{this.Configuration["Enphase:ClientSecret"]}"));
            refreshRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            var response = await client.SendAsync(refreshRequest);

            if (response.IsSuccessStatusCode is false)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Token refresh failed: {response.StatusCode}, {errorContent}.");

                return false;
            }

            // Parse new tokens.
            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenData = JsonDocument.Parse(responseContent).RootElement;

            var newAccessToken = tokenData.GetProperty("access_token").GetString();
            var newRefreshToken = tokenData.TryGetProperty("refresh_token", out var rt) ? rt.GetString() : refreshToken;
            var expiresIn = tokenData.TryGetProperty("expires_in", out var exp) ? exp.GetInt32() : 3600;

            // Update stored tokens.
            var newTokens = new List<AuthenticationToken>
            {
                new AuthenticationToken { Name = "access_token", Value = newAccessToken ?? "" },
                new AuthenticationToken { Name = "refresh_token", Value = newRefreshToken ?? ""},
                new AuthenticationToken { Name = "expires_at", Value = DateTimeOffset.UtcNow.AddSeconds(expiresIn).ToString("o") }
            };

            // Get current authentication result and update tokens.
            var authenticateResult = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (authenticateResult.Succeeded is true)
            {
                if (context.Response.HasStarted is true)
                {
                    // If response has already started, we cannot sign in so cache the access token instead.
                    this.CachedAccessToken = newAccessToken;
                    return false;
                }

                authenticateResult.Properties.StoreTokens(newTokens);
                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    authenticateResult.Principal, authenticateResult.Properties);

                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Token refresh exception: {ex.Message}");
            return false;
        }
    }

    #region System Details API Methods
    public async Task<EnphaseSystemsResponse> GetSystemsAsync()
    {
        // Set to -1 to avoid error message, since system Id is not used in this method.
        var systemId = -1;
        string? apiKey = await this.SetupEnphaseHttpClientAsync(systemId);

        // Add the API key as a query parameter.
        var url = $"{this.EnphaseApiBaseUrl}?key={apiKey}";

        var response = await this.HttpClient.GetFromJsonAsync<EnphaseSystemsResponse>(url);

        return response ?? new EnphaseSystemsResponse();
    }

    public async Task<EnphaseSystemSummaryResponse> GetSystemSummaryAsync(int systemId)
    {
        string? apiKey = await this.SetupEnphaseHttpClientAsync(systemId);

        // Add the API key as a query parameter.
        var url = $"{this.EnphaseApiBaseUrl}/{systemId}/summary?key={apiKey}";

        var response = await this.HttpClient.GetFromJsonAsync<EnphaseSystemSummaryResponse>(url);

        return response ?? new EnphaseSystemSummaryResponse();
    }

    public async Task<EnphaseDevicesResponse> GetSystemDevicesAsync(int systemId)
    {
        string? apiKey = await this.SetupEnphaseHttpClientAsync(systemId);

        // Add the API key as a query parameter.
        var url = $"{this.EnphaseApiBaseUrl}/{systemId}/devices?key={apiKey}";

        var response = await this.HttpClient.GetFromJsonAsync<EnphaseDevicesResponse>(url);

        return response ?? new EnphaseDevicesResponse();
    }

    public async Task<EnphaseMicroinvertersSummaryResponse> GetMicroinvertersSummaryAsync(int systemId)
    {
        string? apiKey = await SetupEnphaseHttpClientAsync(systemId);

        // Add the API key as a query parameter.
        var url = $"{this.EnphaseApiBaseUrl}/inverters_summary_by_envoy_or_site?site_id={systemId}&key={apiKey}";

        // Note: this method is different than the others because the Enphase Documentation is incorrect, so I needed
        // to view the content of the response to find the problem: the last report date is a string, not a long like the others.
        // Also be aware that this endpoint returns a list of summaries, not just one summary. This doesn't make sense,
        // because the system ID is unique, and the response itself includes the array of microinverters,
        // so my response class is the first item in the list, not the entire response from the endpoint.

        try
        {
            var response = await this.HttpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode is false)
            {
                throw new Exception($"API Error {response.StatusCode}: {content}");
            }

            var returnedList = System.Text.Json.JsonSerializer.Deserialize<List<EnphaseMicroinvertersSummaryResponse>>(content);

            var summary = returnedList?.FirstOrDefault();

            return summary ?? new EnphaseMicroinvertersSummaryResponse();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex + " Failed to get microinverter summary.");
            throw;
        }
    }
    #endregion

    #region Site Level Monitoring API Methods
    public async Task<EnphaseEnergyLifetimeResponse> GetEnergyLifetimeAsync(int systemId)
    {
        string? apiKey = await this.SetupEnphaseHttpClientAsync(systemId);

        // Add the API key as a query parameter and ask for all production data.
        var url = $"{this.EnphaseApiBaseUrl}/{systemId}/energy_lifetime?production=all&key={apiKey}";

        var response = await this.HttpClient.GetFromJsonAsync<EnphaseEnergyLifetimeResponse>(url);

        return response ?? new EnphaseEnergyLifetimeResponse();
    }

    public async Task<EnphaseConsumptionLifetimeResponse> GetConsumptionLifetimeAsync(int systemId)
    {
        string? apiKey = await this.SetupEnphaseHttpClientAsync(systemId);

        // Add the API key as a query parameter and ask for all consumption data.
        var url = $"{this.EnphaseApiBaseUrl}/{systemId}/consumption_lifetime?key={apiKey}";

        var response = await this.HttpClient.GetFromJsonAsync<EnphaseConsumptionLifetimeResponse>(url);

        return response ?? new EnphaseConsumptionLifetimeResponse();
    }
    #endregion

    public async Task<EnphaseEnergyImportLifetimeResponse> GetEnergyImportLifetimeAsync(int systemId)
    {
        string? apiKey = await this.SetupEnphaseHttpClientAsync(systemId);

        // Add the API key as a query parameter and ask for all import data.
        var url = $"{this.EnphaseApiBaseUrl}/{systemId}/energy_import_lifetime?key={apiKey}";

        var response = await this.HttpClient.GetFromJsonAsync<EnphaseEnergyImportLifetimeResponse>(url);

        return response ?? new EnphaseEnergyImportLifetimeResponse();
    }

    public async Task<EnphaseEnergyExportLifetimeResponse> GetEnergyExportLifetimeAsync(int systemId)
    {
        string? apiKey = await this.SetupEnphaseHttpClientAsync(systemId);

        // Add the API key as a query parameter and ask for all export data.
        var url = $"{this.EnphaseApiBaseUrl}/{systemId}/energy_export_lifetime?key={apiKey}";

        var response = await this.HttpClient.GetFromJsonAsync<EnphaseEnergyExportLifetimeResponse>(url);

        return response ?? new EnphaseEnergyExportLifetimeResponse();
    }
}
