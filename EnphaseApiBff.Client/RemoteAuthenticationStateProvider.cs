using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace EnphaseApiBff.Client;

public class RemoteAuthenticationStateProvider : AuthenticationStateProvider
{
    private HttpClient HttpClient { get; set; }

    public RemoteAuthenticationStateProvider(HttpClient httpClient)
    {
        this.HttpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var userInfo = await this.HttpClient.GetFromJsonAsync<UserInfo>("api/user");
            if (userInfo?.IsAuthenticated is true)
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, userInfo.Name) };
                claims.AddRange(userInfo.Claims.Select(c => new Claim(c.Key, c.Value)));

                var identity = new ClaimsIdentity(claims, "serverauth");

                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
        }
        catch
        {
            // TODO: Handle or log error
        }

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }
}

public class UserInfo
{
    public bool IsAuthenticated { get; set; }
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, string> Claims { get; set; } = new();
}