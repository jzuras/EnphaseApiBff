using EnphaseApiBff.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace EnphaseApiBff.Client;

internal class Program
{
    static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        builder.Services.AddCascadingAuthenticationState();

        builder.Services.AddAuthorizationCore();

        // Register named HttpClient for API calls.
        builder.Services.AddHttpClient("ServerAPI",
            client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

        // Register default HttpClient (for RemoteAuthenticationStateProvider and Bff Service).
        builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerAPI"));

        // Register auth state provider.
        builder.Services.AddScoped<AuthenticationStateProvider, RemoteAuthenticationStateProvider>();

        builder.Services.AddScoped<IEnphaseBffService, EnphaseBffService>();

        await builder.Build().RunAsync();
    }
}
