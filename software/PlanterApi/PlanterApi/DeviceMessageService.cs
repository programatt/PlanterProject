using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace PlanterApi;

public interface IDeviceMessageService
{
    Task<bool> SaveSentDeviceMessage(SentDeviceMessage message);
}

public class DeviceMessageService : IDeviceMessageService
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<DeviceMessageService> _logger;

    public DeviceMessageService(ApplicationDbContext db, ILogger<DeviceMessageService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<bool> SaveSentDeviceMessage(SentDeviceMessage message)
    {
        try
        {
            await CreateOrUpdateDevice(message);
            await CreateDeviceMessage(message);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to Save DeviceMessage");
            return false;
        }
    }

    private async Task CreateDeviceMessage(SentDeviceMessage message)
    {
        var deviceMessage = DeviceMessage.FromSentDeviceMessage(message);
        _logger.LogInformation("Saving DeviceMessage");
        await _db.DeviceMessages.AddAsync(deviceMessage);    
    }

    #pragma warning disable CA2254
    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Local")]
    private async Task<Device> CreateOrUpdateDevice(SentDeviceMessage message)
    {
        _logger.LogInformation($"Looking Up Device {message.Id}");
        var device = await _db.Devices.SingleOrDefaultAsync(d => d.Id == message.Id);
        if (device == null)
        {
            _logger.LogInformation($"New Device {message.Id} Detected");
            device = new Device { Id = message.Id, CreatedDate = DateTime.UtcNow, LastReceivedTimestamp = message.Timestamp }; 
            await _db.Devices.AddAsync(device);
        }
        else
        {
            _logger.LogInformation($"Updating Device {message.Id} LastReceivedTimestamp {message.Timestamp:O}");
            device.LastReceivedTimestamp = message.Timestamp;
        }

        return device;
    }
    #pragma warning restore CA2254
}