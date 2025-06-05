using EnphaseApiBff.Shared;
using Microsoft.AspNetCore.Components;

namespace EnphaseApiBff.Client.Pages;

public partial class EnphaseSystems
{
    [Inject]
    private IEnphaseBffService EnphaseBffService { get; set; } = null!;

    private EnphaseSystem[] Systems { get; set; } = Array.Empty<EnphaseSystem>();
    private bool IsLoading { get; set; } = true;
    private string? Error { get; set; }

    protected override async Task OnInitializedAsync()
    {
        // Note - this page keeps pre-rendering turned on in order to show how the new
        // RendererInfo.IsInteractive property can be used to determine if the
        // page is being rendered in a pre-rendering context or not. 
        // The property is also referenced in the Razor code, combined with
        // IsLoading to show the loading indicator.

        try
        {
            this.IsLoading = true;
            if (base.RendererInfo.IsInteractive is true)
            {
                // For example purposes only, we only call the API when the page is interactive.
                // Production code would likely just turn off pre-rendering.

                var response = await this.EnphaseBffService.GetSystemsAsync();

                if (response.Systems.Length > 0)
                {
                    this.Systems = response.Systems;
                }
                else
                {
                    this.Systems = Array.Empty<EnphaseSystem>();
                }
            }
        }
        catch (Exception ex)
        {
            this.Error = ex.Message;
        }
        finally
        {
            this.IsLoading = false;
        }
    }
}
