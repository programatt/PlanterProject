using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace PlanterApi;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
#pragma warning disable CS8618
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}
    
    public DbSet<DeviceMessage> DeviceMessages { get; set; }
    public DbSet<Device> Devices { get; set; }
}