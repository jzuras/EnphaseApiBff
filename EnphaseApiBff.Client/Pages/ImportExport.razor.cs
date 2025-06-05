using EnphaseApiBff.Shared;
using Microsoft.AspNetCore.Components;

namespace EnphaseApiBff.Client.Pages;

public partial class ImportExport
{
    [Parameter]
    public int SystemId { get; set; }

    [Inject]
    private IEnphaseBffService EnphaseBffService { get; set; } = null!;

    private EnphaseEnergyImportLifetimeResponse ImportData { get; set; } = new();
    private EnphaseEnergyExportLifetimeResponse ExportData { get; set; } = new();
    private bool IsLoading { get; set; } = true;
    private string? Error { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            this.IsLoading = true;

            // Load both import and export data concurrently
            var importTask = this.EnphaseBffService.GetEnergyImportLifetimeAsync(SystemId);
            var exportTask = this.EnphaseBffService.GetEnergyExportLifetimeAsync(SystemId);

            await Task.WhenAll(importTask, exportTask);

            this.ImportData = await importTask;
            this.ExportData = await exportTask;
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
