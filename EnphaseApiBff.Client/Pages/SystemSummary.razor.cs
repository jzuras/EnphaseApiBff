using EnphaseApiBff.Shared;
using Microsoft.AspNetCore.Components;

namespace EnphaseApiBff.Client.Pages;

public partial class SystemSummary
{
    [Parameter]
    public int SystemId { get; set; }

    [Inject]
    private IEnphaseBffService EnphaseBffService { get; set; } = null!;

    private EnphaseSystemSummaryResponse Summary { get; set; } = new();
    private bool IsLoading { get; set; } = true;
    private string? Error { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            this.IsLoading = true;
            this.Summary = await this.EnphaseBffService.GetSystemSummaryAsync(SystemId);
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
