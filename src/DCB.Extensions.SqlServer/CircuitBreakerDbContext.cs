using DCB.Helpers;
using Microsoft.EntityFrameworkCore;

namespace DCB.Extensions.SqlServer;

public class CircuitBreakerDbContext:DbContext
{
    public CircuitBreakerDbContext(DbContextOptions<CircuitBreakerDbContext> ops):base(ops)
    {
    }
    
    public DbSet<CircuitBreakerDataModel> CircuitBreakers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) 
        => modelBuilder.Entity<CircuitBreakerDataModel>(ops => ops.HasKey(x => x.Name));

}