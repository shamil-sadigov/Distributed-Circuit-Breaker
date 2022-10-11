using Microsoft.EntityFrameworkCore;

namespace Storage.SqlServer;

// TODO: Replace EF with ADO.NET or Dapper

public class CircuitBreakerDbContext : DbContext
{
    public CircuitBreakerDbContext(DbContextOptions<CircuitBreakerDbContext> ops) : base(ops)
    {
    }

    public DbSet<CircuitBreakerDataModel> CircuitBreakers { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CircuitBreakerDataModel>(ops => ops.HasKey(x => x.Name));
    }
}