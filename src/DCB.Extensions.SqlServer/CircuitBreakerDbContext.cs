using DCB.Helpers;
using Microsoft.EntityFrameworkCore;

namespace DCB.Extensions.SqlServer;

public class CircuitBreakerDbContext:DbContext
{
    private readonly string _connectionString;

    public CircuitBreakerDbContext(string connectionString)
    {
        _connectionString?.ThrowIfNull();
        _connectionString = connectionString;
    }
    
    public DbSet<CircuitBreakerDataModel> CircuitBreakers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) 
        => modelBuilder.Entity<CircuitBreakerDataModel>(ops => ops.HasKey(x => x.Name));

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
        => optionsBuilder.UseSqlServer(_connectionString);
}