namespace PlanterApi;

public static class Endpoints
{
    public static async Task<IResult> SaveDeviceMessage(IDeviceMessageService service, SentDeviceMessage message)
        => await service.SaveSentDeviceMessage(message)
            ? Results.Ok()
            : Results.Problem("Unable to Save DeviceMessage"); 
}