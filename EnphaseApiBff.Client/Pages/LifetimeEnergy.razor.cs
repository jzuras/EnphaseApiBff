using EnphaseApiBff.Shared;
using Microsoft.AspNetCore.Components;

namespace EnphaseApiBff.Client.Pages;

public partial class LifetimeEnergy
{
    [Parameter]
    public int SystemId { get; set; }

    [Inject]
    private IEnphaseBffService EnphaseBffService { get; set; } = null!;

    private EnphaseEnergyLifetimeResponse EnergyData { get; set; } = new();
    private EnphaseConsumptionLifetimeResponse ConsumptionData { get; set; } = new();

    private bool IsLoading { get; set; } = true;
    private string? Error { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            this.IsLoading = true;
            this.EnergyData = await this.EnphaseBffService.GetEnergyLifetimeAsync(SystemId);
            this.ConsumptionData = await this.EnphaseBffService.GetConsumptionLifetimeAsync(SystemId);
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
