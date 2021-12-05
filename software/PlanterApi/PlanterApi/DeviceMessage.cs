using System.Diagnostics.CodeAnalysis;

namespace PlanterApi;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class DeviceMessage : SentDeviceMessage
{
    public Guid DeviceId { get; set; }
    public Device Device { get; set; } = null!;
    public DateTime ProcessedTimestamp { get; set; }

    public static DeviceMessage FromSentDeviceMessage(SentDeviceMessage message)
        => new()
        {
            Id = Guid.NewGuid(),
            DeviceId = message.Id,
            AirHumidityPercentage = message.AirHumidityPercentage,
            AirTemperatureCelsius = message.AirTemperatureCelsius,
            SoilMoisturePercentage = message.SoilMoisturePercentage,
            SoilTemperatureCelsius = message.SoilTemperatureCelsius,
            ProcessedTimestamp = DateTime.UtcNow,
            Timestamp = message.Timestamp,
            WaterPumpOn = message.WaterPumpOn,
            LightSourceOn = message.LightSourceOn
        };
}