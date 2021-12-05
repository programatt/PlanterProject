using System.Diagnostics.CodeAnalysis;

namespace PlanterApi;

[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
public class SentDeviceMessage
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public int AirTemperatureCelsius { get; set; }
    public int AirHumidityPercentage { get; set; }
    public int SoilTemperatureCelsius { get; set; }
    public int SoilMoisturePercentage { get; set; }
    public bool WaterPumpOn { get; set; }
    public bool LightSourceOn { get; set; }
}