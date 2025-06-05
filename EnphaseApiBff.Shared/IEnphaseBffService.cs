namespace EnphaseApiBff.Shared;

public interface IEnphaseBffService
{
    Task<EnphaseSystemsResponse> GetSystemsAsync();
    Task<EnphaseSystemSummaryResponse> GetSystemSummaryAsync(int systemId);
    Task<EnphaseDevicesResponse> GetSystemDevicesAsync(int systemId);
    Task<EnphaseMicroinvertersSummaryResponse> GetMicroinvertersSummaryAsync(int systemId);

    Task<EnphaseEnergyLifetimeResponse> GetEnergyLifetimeAsync(int systemId);
    Task<EnphaseConsumptionLifetimeResponse> GetConsumptionLifetimeAsync(int systemId);
    Task<EnphaseEnergyImportLifetimeResponse> GetEnergyImportLifetimeAsync(int systemId);
    Task<EnphaseEnergyExportLifetimeResponse> GetEnergyExportLifetimeAsync(int systemId);
}
