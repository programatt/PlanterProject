using PlanterApi;

var builder = WebApplication.CreateBuilder(args);

builder.AddPostgres<ApplicationDbContext>("DefaultConnection");
builder.Services.AddScoped<IDeviceMessageService, DeviceMessageService>();

var app = builder.Build();

app.MapPost("/devicemessage", Endpoints.SaveDeviceMessage);
app.Run();