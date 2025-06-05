using EnphaseApiBff.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnphaseApiBff.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/enphase")]
public class EnphaseBffController : Controller
{
    private IEnphaseBffService EnphaseBffService { get; set; } = null!;

    public EnphaseBffController(IEnphaseBffService enphaseBffService)
    {
        this.EnphaseBffService = enphaseBffService;
    }

    [HttpGet("systems")]
    public async Task<ActionResult<EnphaseSystemsResponse>> GetSystemsAsync()
    {
        try
        {
            var response = await this.EnphaseBffService.GetSystemsAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpGet("systems/{systemId}/devices")]
    public async Task<ActionResult<EnphaseDevicesResponse>> GetSystemDevices(int systemId)
    {
        try
        {
            var response = await this.EnphaseBffService.GetSystemDevicesAsync(systemId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpGet("systems/{systemId}/summary")]
    public async Task<ActionResult<EnphaseDevicesResponse>> GetSystemSummaryAsync(int systemId)
    {
        try
        {
            var response = await this.EnphaseBffService.GetSystemSummaryAsync(systemId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpGet("systems/{systemId}/microinverters")]
    public async Task<ActionResult<EnphaseMicroinvertersSummaryResponse>> GetMicroinvertersSummaryAsync(int systemId)
    {
        try
        {
            var response = await this.EnphaseBffService.GetMicroinvertersSummaryAsync(systemId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpGet("systems/{systemId}/energy-lifetime")]
    public async Task<ActionResult<EnphaseEnergyLifetimeResponse>> GetEnergyLifetimeAsync(int systemId)
    {
        try
        {
            var response = await this.EnphaseBffService.GetEnergyLifetimeAsync(systemId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpGet("systems/{systemId}/consumption-lifetime")]
    public async Task<ActionResult<EnphaseConsumptionLifetimeResponse>> GetConsumptionLifetimeAsync(int systemId)
    {
        try
        {
            var response = await this.EnphaseBffService.GetConsumptionLifetimeAsync(systemId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpGet("systems/{systemId}/energy-import-lifetime")]
    public async Task<ActionResult<EnphaseEnergyImportLifetimeResponse>> GetEnergyImportLifetimeAsync(int systemId)
    {
        try
        {
            var response = await this.EnphaseBffService.GetEnergyImportLifetimeAsync(systemId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }

    [HttpGet("systems/{systemId}/energy-export-lifetime")]
    public async Task<ActionResult<EnphaseEnergyExportLifetimeResponse>> GetEnergyExportLifetimeAsync(int systemId)
    {
        try
        {
            var response = await this.EnphaseBffService.GetEnergyExportLifetimeAsync(systemId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Problem(ex.Message);
        }
    }
}
