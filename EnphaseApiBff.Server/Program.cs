using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using EnphaseApiBff.Client;
using EnphaseApiBff.Shared;
using EnphaseApiBff.Server.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace EnphaseApiBff.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        // Enable authentication state across render modes.
        builder.Services.AddAuthorizationCore();
        builder.Services.AddCascadingAuthenticationState();

        builder.Services.AddHttpClient();

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddScoped<IEnphaseBffService, EnphaseBffService>();

        builder.Services.AddControllers();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie(options =>
        {
            options.LoginPath = "/login";
            options.AccessDeniedPath = "/";
            options.ExpireTimeSpan = TimeSpan.FromDays(365);
            options.Cookie.SameSite = SameSiteMode.Lax; // needed for Blazor WebAssembly to work correctly.
        });

        // Add authorization services.
        builder.Services.AddAuthorization();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment() is true)
        {
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        // Add authentication and authorization middleware
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

        app.MapControllers();

        // Used by RemoteAuthenticationStateProvider.GetAuthenticationStateAsync().
        app.MapGet("/api/user", (ClaimsPrincipal user) =>
        {
            return new UserInfo
            {
                IsAuthenticated = user.Identity?.IsAuthenticated ?? false,
                Name = user.Identity?.Name ?? string.Empty,
                Claims = user.Claims.ToDictionary(c => c.Type, c => c.Value)
            };
        }).RequireAuthorization();

        app.MapGet("/login", (HttpContext context, IConfiguration config) =>
        {
            var clientId = config["Enphase:ClientId"];
            var redirectUri = $"{context.Request.Scheme}://{context.Request.Host}/signin-enphase";

            // Get the returnUrl from the query string.
            var returnUrl = context.Request.Query["returnUrl"].ToString();
            if (string.IsNullOrEmpty(returnUrl) is true)
            {
                returnUrl = "/";
            }

            // Add returnUrl as the state parameter.
            var authUrl = $"https://api.enphaseenergy.com/oauth/authorize" +
                         $"?response_type=code" +
                         $"&client_id={clientId}" +
                         $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                         $"&state={Uri.EscapeDataString(returnUrl)}";

            return Results.Redirect(authUrl);
        });

        app.MapGet("/signin-enphase", async (HttpContext context, IConfiguration config) =>
        {
            // Get the authorization code from the query string.
            var code = context.Request.Query["code"];

            if (string.IsNullOrEmpty(code) is true)
            {
                return Results.BadRequest("Authorization code not received.");
            }

            // Exchange code for token using Enphase's requirements.
            using var client = new HttpClient();
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.enphaseenergy.com/oauth/token");

            // Add required parameters.
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code",
                ["code"] = code!,
                ["redirect_uri"] = $"{context.Request.Scheme}://{context.Request.Host}/signin-enphase"
            });
            tokenRequest.Content = content;

            // Add credentials as basic auth header.
            var credentials = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{config["Enphase:ClientId"]}:{config["Enphase:ClientSecret"]}"));
            tokenRequest.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            var response = await client.SendAsync(tokenRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode is false)
            {
                return Results.Problem($"Token exchange failed: {responseContent}.");
            }

            // Process successful response.
            var tokenData = JsonDocument.Parse(responseContent).RootElement;

            var accessToken = tokenData.GetProperty("access_token").GetString();
            var refreshToken = tokenData.TryGetProperty("refresh_token", out var rt) ? rt.GetString() : null;
            var expiresIn = tokenData.TryGetProperty("expires_in", out var exp) ? exp.GetInt32() : 3600;

            // Create authentication claims.
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "enphase_user"),
            };

            // Create identity and principal.
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Create token list.
            var tokens = new List<AuthenticationToken>
            {
                new AuthenticationToken { Name = "access_token", Value = accessToken ?? string.Empty }
            };

            // Add refresh token if available.
            if (string.IsNullOrEmpty(refreshToken) is false)
            {
                tokens.Add(new AuthenticationToken { Name = "refresh_token", Value = refreshToken });
            }

            // Add expiration.
            tokens.Add(new AuthenticationToken
            {
                Name = "expires_at",
                Value = DateTimeOffset.UtcNow.AddSeconds(expiresIn).ToString("o")
            });

            // Store tokens.
            var props = new AuthenticationProperties();
            props.StoreTokens(tokens);
            props.IsPersistent = true;

            // Update sign-in to include properties.
            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);

            // After successful sign-in, check for returnUrl.
            var returnUrl = context.Request.Query["state"].ToString();

            // Validate and use returnUrl.
            if (string.IsNullOrEmpty(returnUrl) is false &&
                (returnUrl.StartsWith('/') is true && returnUrl.StartsWith("//") is false && returnUrl.Contains(':') is false))
            {
                return Results.Redirect(returnUrl);
            }

            // Default to home page if no valid returnUrl.
            return Results.Redirect("/");
        });

        // Add a logout endpoint.
        app.MapGet("/logout", async (HttpContext context) =>
        {
            // Sign out the user.
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirect to home page after logout.
            return Results.Redirect("/");
        });

        app.Run();
    }
}
