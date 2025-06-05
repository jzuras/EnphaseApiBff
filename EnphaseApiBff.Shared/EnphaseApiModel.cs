using System.Text.Json.Serialization;

namespace EnphaseApiBff.Shared;

#region System Classes
public class EnphaseSystemsResponse
{
    [JsonPropertyName("systems")]
    public EnphaseSystem[] Systems { get; set; } = Array.Empty<EnphaseSystem>();
}

public class EnphaseSystem
{
    [JsonPropertyName("system_id")]
    public int SystemId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("public_name")]
    public string PublicName { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("timezone")]
    public string Timezone { get; set; } = string.Empty;

    [JsonPropertyName("address")]
    public EnphaseAddress Address { get; set; } = new EnphaseAddress();

    [JsonPropertyName("connection_type")]
    public string ConnectionType { get; set; } = string.Empty;

    [JsonPropertyName("energy_lifetime")]
    public long EnergyLifetime { get; set; }

    [JsonPropertyName("energy_today")]
    public long EnergyToday { get; set; }

    [JsonPropertyName("system_size")]
    public long SystemSize { get; set; }

    [JsonPropertyName("last_report_at")]
    public long LastReportAt { get; set; }

    [JsonPropertyName("last_energy_at")]
    public long LastEnergyAt { get; set; }

    [JsonPropertyName("operational_at")]
    public long OperationalAt { get; set; }

    [JsonPropertyName("reference")]
    public long Reference { get; set; }

    // This one was coming back as "null" in the API response,
    // even though the API documentation says it should be a number.
    // Nullable long did not parse correctly.
    //[JsonPropertyName("other_references")]
    //public long OtherReferences { get; set; }

    // These last two are shown as "null" in the API response.
    //[JsonPropertyName("attachment_type")]
    //public long AttachmentType { get; set; }

    //[JsonPropertyName("interconnect_date")]
    //public long InterconnectDate { get; set; }

}

public class EnphaseAddress
{
    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;

    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;

    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;

    [JsonPropertyName("postal_code")]
    public string PostalCode { get; set; } = string.Empty;
}

public class EnphaseSystemSummaryResponse
{
    [JsonPropertyName("system_id")]
    public int SystemId { get; set; }

    [JsonPropertyName("current_power")]
    public int CurrentPower { get; set; }

    [JsonPropertyName("energy_lifetime")]
    public long EnergyLifetime { get; set; }

    [JsonPropertyName("energy_today")]
    public int EnergyToday { get; set; }

    [JsonPropertyName("last_interval_end_at")]
    public long LastIntervalEndAt { get; set; }

    [JsonPropertyName("last_report_at")]
    public long LastReportAt { get; set; }

    [JsonPropertyName("modules")]
    public int Modules { get; set; }

    [JsonPropertyName("operational_at")]
    public long? OperationalAt { get; set; }

    [JsonPropertyName("size_w")]
    public int SizeW { get; set; }

    [JsonPropertyName("source")]
    public string Source { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("summary_date")]
    public string SummaryDate { get; set; } = string.Empty;

    [JsonPropertyName("battery_charge_w")]
    public int? BatteryChargeW { get; set; }

    [JsonPropertyName("battery_discharge_w")]
    public int? BatteryDischargeW { get; set; }

    [JsonPropertyName("battery_capacity_wh")]
    public int? BatteryCapacityWh { get; set; }
}
#endregion

#region Device Classes
public class EnphaseDevicesResponse
{
    [JsonPropertyName("system_id")]
    public int SystemId { get; set; }

    [JsonPropertyName("total_devices")]
    public int TotalDevices { get; set; }

    [JsonPropertyName("items")]
    public string Items { get; set; } = string.Empty;

    [JsonPropertyName("devices")]
    public EnphaseDevices Devices { get; set; } = new EnphaseDevices();
}

public class EnphaseDevices
{
    [JsonPropertyName("micros")]
    public List<EnphaseMicroinverter> Micros { get; set; } = new List<EnphaseMicroinverter>();

    [JsonPropertyName("meters")]
    public List<EnphaseMeter> Meters { get; set; } = new List<EnphaseMeter>();

    [JsonPropertyName("gateways")]
    public List<EnphaseGateway> Gateways { get; set; } = new List<EnphaseGateway>();

    [JsonPropertyName("q_relays")]
    public List<EnphaseQRelay> QRelays { get; set; } = new List<EnphaseQRelay>();

    [JsonPropertyName("acbs")]
    public List<EnphaseAcb> Acbs { get; set; } = new List<EnphaseAcb>();

    [JsonPropertyName("encharges")]
    public List<EnphaseEncharge> Encharges { get; set; } = new List<EnphaseEncharge>();

    [JsonPropertyName("enpowers")]
    public List<EnphaseEnpower> Enpowers { get; set; } = new List<EnphaseEnpower>();

    [JsonPropertyName("ev_chargers")]
    public List<EnphaseEvCharger> EvChargers { get; set; } = new List<EnphaseEvCharger>();
}

// Base class for common device properties
public class EnphaseDevice
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("last_report_at")]
    public long LastReportAt { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("serial_number")]
    public string SerialNumber { get; set; } = string.Empty;

    [JsonPropertyName("part_number")]
    public string PartNumber { get; set; } = string.Empty;

    [JsonPropertyName("sku")]
    public string? Sku { get; set; }

    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("active")]
    public bool Active { get; set; }

    [JsonPropertyName("product_name")]
    public string ProductName { get; set; } = string.Empty;
}

public class EnphaseMicroinverter : EnphaseDevice
{
}

public class EnphaseMeter : EnphaseDevice
{
    [JsonPropertyName("state")]
    public string State { get; set; } = string.Empty;

    [JsonPropertyName("config_type")]
    public string ConfigType { get; set; } = string.Empty;
}

public class EnphaseGateway : EnphaseDevice
{
    [JsonPropertyName("emu_sw_version")]
    public string EmuSwVersion { get; set; } = string.Empty;

    [JsonPropertyName("cellular_modem")]
    public EnphaseCellularModem? CellularModem { get; set; }
}

public class EnphaseCellularModem
{
    [JsonPropertyName("imei")]
    public string Imei { get; set; } = string.Empty;

    [JsonPropertyName("part_num")]
    public string PartNum { get; set; } = string.Empty;

    [JsonPropertyName("sku")]
    public string Sku { get; set; } = string.Empty;

    [JsonPropertyName("plan_start_date")]
    public long PlanStartDate { get; set; }

    [JsonPropertyName("plan_end_date")]
    public long PlanEndDate { get; set; }
}

public class EnphaseQRelay : EnphaseDevice
{
}

public class EnphaseAcb : EnphaseDevice
{
}

public class EnphaseEncharge : EnphaseDevice
{
}

public class EnphaseEnpower : EnphaseDevice
{
}

public class EnphaseEvCharger : EnphaseDevice
{
    [JsonPropertyName("firmware")]
    public string Firmware { get; set; } = string.Empty;
}
#endregion

#region Microinverters Summary Classes
public class EnphaseMicroinvertersSummaryResponse
{
    [JsonPropertyName("signal_strength")]
    public int SignalStrength { get; set; }

    [JsonPropertyName("micro_inverters")]
    public List<EnphaseMicroinvertersSummary> MicroInverters { get; set; } = new List<EnphaseMicroinvertersSummary>();
}

public class EnphaseMicroinvertersSummary
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("serial_number")]
    public string SerialNumber { get; set; } = string.Empty;

    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("part_number")]
    public string PartNumber { get; set; } = string.Empty;

    [JsonPropertyName("sku")]
    public string Sku { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("power_produced")]
    public EnphasePowerValue PowerProduced { get; set; } = new EnphasePowerValue();

    [JsonPropertyName("proc_load")]
    public string ProcLoad { get; set; } = string.Empty;

    [JsonPropertyName("param_table")]
    public string ParamTable { get; set; } = string.Empty;

    [JsonPropertyName("envoy_serial_number")]
    public string EnvoySerialNumber { get; set; } = string.Empty;

    [JsonPropertyName("energy")]
    public EnphaseEnergyValue Energy { get; set; } = new EnphaseEnergyValue();

    [JsonPropertyName("grid_profile")]
    public string GridProfile { get; set; } = string.Empty;

    [JsonPropertyName("last_report_date")]
    public DateTimeOffset LastReportDate { get; set; }
}

public class EnphasePowerValue
{
    [JsonPropertyName("value")]
    public int Value { get; set; }

    [JsonPropertyName("units")]
    public string Units { get; set; } = string.Empty;

    [JsonPropertyName("precision")]
    public int Precision { get; set; }
}

public class EnphaseEnergyValue
{
    [JsonPropertyName("value")]
    public int Value { get; set; }

    [JsonPropertyName("units")]
    public string Units { get; set; } = string.Empty;

    [JsonPropertyName("precision")]
    public int Precision { get; set; }
}
#endregion

#region Site Level Production and Consumption Monitoring Classes
public class EnphaseEnergyLifetimeResponse
{
    [JsonPropertyName("system_id")]
    public int SystemId { get; set; }

    [JsonPropertyName("start_date")]
    public string StartDate { get; set; } = string.Empty;

    [JsonPropertyName("meter_start_date")]
    public string MeterStartDate { get; set; } = string.Empty;

    [JsonPropertyName("production")]
    public List<long> Production { get; set; } = new List<long>();

    [JsonPropertyName("micro_production")]
    public List<long> MicroProduction { get; set; } = new List<long>();

    [JsonPropertyName("meter_production")]
    public List<long> MeterProduction { get; set; } = new List<long>();

    [JsonPropertyName("meta")]
    public EnphaseLifetimeMeta Meta { get; set; } = new EnphaseLifetimeMeta();
}

public class EnphaseConsumptionLifetimeResponse
{
    [JsonPropertyName("system_id")]
    public int SystemId { get; set; }

    [JsonPropertyName("start_date")]
    public string StartDate { get; set; } = string.Empty;

    [JsonPropertyName("consumption")]
    public List<long> Consumption { get; set; } = new List<long>();

    [JsonPropertyName("meta")]
    public EnphaseLifetimeMeta Meta { get; set; } = new EnphaseLifetimeMeta();
}

public class EnphaseLifetimeMeta
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("last_report_at")]
    public long LastReportAt { get; set; }

    [JsonPropertyName("last_energy_at")]
    public long LastEnergyAt { get; set; }

    [JsonPropertyName("operational_at")]
    public long OperationalAt { get; set; }
}
#endregion

#region Energy Import and Export Classes
public class EnphaseEnergyImportLifetimeResponse
{
    [JsonPropertyName("system_id")]
    public int SystemId { get; set; }

    [JsonPropertyName("start_date")]
    public string StartDate { get; set; } = string.Empty;

    [JsonPropertyName("import")]
    public List<long> Import { get; set; } = new List<long>();

    [JsonPropertyName("meta")]
    public EnphaseLifetimeMeta Meta { get; set; } = new EnphaseLifetimeMeta();
}

public class EnphaseEnergyExportLifetimeResponse
{
    [JsonPropertyName("system_id")]
    public int SystemId { get; set; }

    [JsonPropertyName("start_date")]
    public string StartDate { get; set; } = string.Empty;

    [JsonPropertyName("export")]
    public List<long> Export { get; set; } = new List<long>();

    [JsonPropertyName("meta")]
    public EnphaseLifetimeMeta Meta { get; set; } = new EnphaseLifetimeMeta();
}
#endregion
