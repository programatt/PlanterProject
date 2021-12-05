using Microsoft.EntityFrameworkCore;

namespace PlanterApi;

public static class WebApplicationBuilderExtensions
{
    public static void AddPostgres<T>(this WebApplicationBuilder builder, string connectionStringName) where T : DbContext
    {
        builder.Services.AddDbContext<T>(options
            => options.UseNpgsql(builder.Configuration.GetConnectionString(connectionStringName))
        );
    }
}