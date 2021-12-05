using System;
using FluentAssertions;
using Xunit;

namespace PlanterApi.Tests;

public class DeviceMessageTests
{
    [Fact]
    public void FromSentDeviceMessage_ShouldCreateDeviceMessageCorrectly()
    {
        var message = new SentDeviceMessage
        {
            Id = Guid.NewGuid(),
            Timestamp = DateTime.UtcNow,
            AirHumidityPercentage = 1,
            AirTemperatureCelsius = 2,
            LightSourceOn = true,
            SoilMoisturePercentage = 3,
            SoilTemperatureCelsius = 4,
            WaterPumpOn = true
        };

        var deviceMessage = DeviceMessage.FromSentDeviceMessage(message);

        deviceMessage.Id.Should().NotBe(Guid.Empty);
        deviceMessage.ProcessedTimestamp.Should().NotBe(DateTime.MinValue);
        deviceMessage.ProcessedTimestamp.Should().NotBe(DateTime.MaxValue);
        deviceMessage.DeviceId.Should().Be(message.Id);
        deviceMessage.AirHumidityPercentage.Should().Be(message.AirHumidityPercentage);
        deviceMessage.AirTemperatureCelsius.Should().Be(message.AirTemperatureCelsius);
        deviceMessage.LightSourceOn.Should().Be(message.LightSourceOn);
        deviceMessage.SoilMoisturePercentage.Should().Be(message.SoilMoisturePercentage);
        deviceMessage.SoilTemperatureCelsius.Should().Be(message.SoilTemperatureCelsius);
        deviceMessage.WaterPumpOn.Should().Be(message.WaterPumpOn);
    }
}