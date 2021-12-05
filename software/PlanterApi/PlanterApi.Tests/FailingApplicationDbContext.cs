using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PlanterApi.Tests;

public class FailingApplicationDbContext : ApplicationDbContext
{
    public FailingApplicationDbContext():base(new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options) { } 
    public override Task<int> SaveChangesAsync(CancellationToken token = default) => throw new Exception("You done goofed");
}
 
