using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace PlanterApi.Tests;

public class DeviceMessageServiceTests
{
   
    private readonly Mock<ILogger<DeviceMessageService>> _logger;
    private ApplicationDbContext _db;
    private DeviceMessageService _service;

    public DeviceMessageServiceTests()
    {
        _logger = new Mock<ILogger<DeviceMessageService>>();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _db = new ApplicationDbContext(options);
        _service = new DeviceMessageService(_db, _logger.Object);
    }
    
    [Fact]
    public async Task SaveDeviceMessage_ShouldReturnTrueAndSaveDataCorrectly()
    {
        var id = Guid.NewGuid();
        var timestamp = DateTime.UtcNow;
        await _db.Devices.AddAsync(new Device {Id = id, CreatedDate = DateTime.UtcNow, LastReceivedTimestamp = timestamp});
        await _db.SaveChangesAsync();
        var message = new SentDeviceMessage
        {
            Id = id,
            Timestamp = timestamp,
            AirHumidityPercentage = 1,
            AirTemperatureCelsius = 2,
            LightSourceOn = true,
            SoilMoisturePercentage = 3,
            SoilTemperatureCelsius = 4,
            WaterPumpOn = true
        };

        (await _service.SaveSentDeviceMessage(message))
            .Should()
            .BeTrue();

        var deviceMessage = await _db.DeviceMessages.FirstOrDefaultAsync();
        deviceMessage!.Id.Should().NotBe(Guid.Empty);
        deviceMessage.DeviceId.Should().Be(id);
        deviceMessage.Timestamp.Should().Be(message.Timestamp);
        deviceMessage.ProcessedTimestamp.Should().NotBe(DateTime.MinValue);
        deviceMessage.AirHumidityPercentage.Should().Be(message.AirHumidityPercentage);
        deviceMessage.AirTemperatureCelsius.Should().Be(message.AirTemperatureCelsius);
        deviceMessage.LightSourceOn.Should().Be(message.LightSourceOn);
        deviceMessage.SoilMoisturePercentage.Should().Be(message.SoilMoisturePercentage);
        deviceMessage.SoilTemperatureCelsius.Should().Be(message.SoilTemperatureCelsius);
        deviceMessage.WaterPumpOn.Should().Be(message.WaterPumpOn);

        var device = await _db.Devices.FirstOrDefaultAsync();
        device!.Id.Should().NotBe(Guid.Empty);
        device.CreatedDate.Should().NotBe(DateTime.MinValue);
        device.LastReceivedTimestamp.Should().Be(timestamp);
    }

    [Fact]
    public async Task SaveDeviceMessage_ShouldCreateDeviceIfItDoesNotExist()
    {
        var id = Guid.NewGuid();
        (await _db.Devices.AnyAsync(d => d.Id == id)).Should().BeFalse();
        var message = new SentDeviceMessage { Id = id };

        await _service.SaveSentDeviceMessage(message);
        
        (await _db.Devices.AnyAsync(d => d.Id == id)).Should().BeTrue();
    }
    
    #pragma warning disable CS8602
    [Fact]
    public async Task SaveDeviceMessage_ShouldUpdateDeviceLastReceivedTimestampIfItExists()
    {
        var id = Guid.NewGuid();
        var timestamp = DateTime.UtcNow;
        await _db.Devices.AddAsync(new Device {Id = id, CreatedDate = DateTime.UtcNow, LastReceivedTimestamp = timestamp.AddDays(-1)});
        await _db.SaveChangesAsync();
        var message = new SentDeviceMessage { Id = id, Timestamp = timestamp};

        await _service.SaveSentDeviceMessage(message);

        (await _db.Devices.SingleOrDefaultAsync(d => d.Id == id))
            .LastReceivedTimestamp
            .Should()
            .Be(timestamp);
    }
    #pragma warning restore CS8602

    [Fact]
    public async Task SaveDeviceMessage_ShouldReturnFalseIfException()
    {
        _db = new FailingApplicationDbContext();
        _service = new DeviceMessageService(_db, _logger.Object);
        var message = new SentDeviceMessage { Id = Guid.NewGuid() };

        (await _service.SaveSentDeviceMessage(message))
            .Should()
            .BeFalse(); 
    }
}