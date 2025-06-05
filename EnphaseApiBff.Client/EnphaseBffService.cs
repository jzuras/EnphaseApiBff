using System.Net.Http.Json;
using EnphaseApiBff.Shared;

namespace EnphaseApiBff.Client;

public class EnphaseBffService : IEnphaseBffService
{
    private HttpClient HttpClient { get; set; }

    public EnphaseBffService(HttpClient httpClient)
    {
        this.HttpClient = httpClient;
    }

    public async Task<EnphaseSystemsResponse> GetSystemsAsync()
    {
        var response = await this.HttpClient.GetFromJsonAsync<EnphaseSystemsResponse>($"/api/enphase/systems");
        return response ?? new EnphaseSystemsResponse();
    }

    public async Task<EnphaseDevicesResponse> GetSystemDevicesAsync(int systemId)
    {
        var response = await this.HttpClient.GetFromJsonAsync<EnphaseDevicesResponse>($"/api/enphase/systems/{systemId}/devices");
        return response ?? new EnphaseDevicesResponse();
    }

    public async Task<EnphaseSystemSummaryResponse> GetSystemSummaryAsync(int systemId)
    {
        var response = await this.HttpClient.GetFromJsonAsync<EnphaseSystemSummaryResponse>($"/api/enphase/systems/{systemId}/summary");
        return response ?? new EnphaseSystemSummaryResponse();
    }

    public async Task<EnphaseMicroinvertersSummaryResponse> GetMicroinvertersSummaryAsync(int systemId)
    {
        var response = await this.HttpClient.GetFromJsonAsync<EnphaseMicroinvertersSummaryResponse>($"/api/enphase/systems/{systemId}/microinverters");
        return response ?? new EnphaseMicroinvertersSummaryResponse();
    }

    public async Task<EnphaseEnergyLifetimeResponse> GetEnergyLifetimeAsync(int systemId)
    {
        var response = await this.HttpClient.GetFromJsonAsync<EnphaseEnergyLifetimeResponse>($"/api/enphase/systems/{systemId}/energy-lifetime");
        return response ?? new EnphaseEnergyLifetimeResponse();
    }

    public async Task<EnphaseConsumptionLifetimeResponse> GetConsumptionLifetimeAsync(int systemId)
    {
        var response = await this.HttpClient.GetFromJsonAsync<EnphaseConsumptionLifetimeResponse>($"/api/enphase/systems/{systemId}/consumption-lifetime");
        return response ?? new EnphaseConsumptionLifetimeResponse();
    }

    public async Task<EnphaseEnergyImportLifetimeResponse> GetEnergyImportLifetimeAsync(int systemId)
    {
        var response = await this.HttpClient.GetFromJsonAsync<EnphaseEnergyImportLifetimeResponse>($"/api/enphase/systems/{systemId}/energy-import-lifetime");
        return response ?? new EnphaseEnergyImportLifetimeResponse();
    }

    public async Task<EnphaseEnergyExportLifetimeResponse> GetEnergyExportLifetimeAsync(int systemId)
    {
        var response = await this.HttpClient.GetFromJsonAsync<EnphaseEnergyExportLifetimeResponse>($"/api/enphase/systems/{systemId}/energy-export-lifetime");
        return response ?? new EnphaseEnergyExportLifetimeResponse();
    }
}
