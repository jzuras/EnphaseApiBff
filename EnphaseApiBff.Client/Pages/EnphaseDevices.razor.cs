using EnphaseApiBff.Shared;
using Microsoft.AspNetCore.Components;

namespace EnphaseApiBff.Client.Pages;

public partial class EnphaseDevices
{
    [Parameter]
    public int SystemId { get; set; }

    [Inject]
    private IEnphaseBffService EnphaseBffService { get; set; } = null!;

    public string ActiveTab { get; set; } = "micros";

    private EnphaseDevicesResponse Devices { get; set; } = new();
    private EnphaseMicroinvertersSummaryResponse Summary { get; set; } = new();
    private bool IsLoading { get; set; } = true;
    private string? Error { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            this.IsLoading = true;
            this.Devices = await this.EnphaseBffService.GetSystemDevicesAsync(SystemId);
            this.Summary = await this.EnphaseBffService.GetMicroinvertersSummaryAsync(SystemId);
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

    private void SetActiveTab(string tabId)
    {
        this.ActiveTab = tabId;
    }
}
