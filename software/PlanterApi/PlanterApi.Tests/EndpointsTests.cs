using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace PlanterApi.Tests;

public class EndpointsTests
{
    private readonly Mock<IDeviceMessageService> _service;
    private readonly HttpContext _httpContext;

    public EndpointsTests()
    {
        _service = new Mock<IDeviceMessageService>();
        _httpContext = new DefaultHttpContext
        {
            RequestServices = new ServiceCollection().AddLogging().BuildServiceProvider(),
            Response = {Body = new MemoryStream()}
        };
    }
    
    [Fact]
    public async Task SaveSentDeviceMessage_ShouldReturnOkIfSuccessful()
    {
        _service.Setup(s => s.SaveSentDeviceMessage(It.IsAny<SentDeviceMessage>()))
            .ReturnsAsync(true);

        var result = await Endpoints.SaveDeviceMessage(_service.Object, new SentDeviceMessage());
        await result.ExecuteAsync(_httpContext);

        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
    }    
    
    [Fact]
    public async Task SaveSentDeviceMessage_ShouldReturn500IfSuccessful()
    {
        _service.Setup(s => s.SaveSentDeviceMessage(It.IsAny<SentDeviceMessage>()))
            .ReturnsAsync(false);

        var result = await Endpoints.SaveDeviceMessage(_service.Object, new SentDeviceMessage());
        await result.ExecuteAsync(_httpContext);

        _httpContext.Response.Body.Position = 0;
        var body = await JsonSerializer.DeserializeAsync<ProblemDetails>(_httpContext.Response.Body, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        _httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        body!.Detail.Should().Be("Unable to Save DeviceMessage");
    }
}