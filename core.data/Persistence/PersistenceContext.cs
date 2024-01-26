using core.data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace core.data.Persistence;

public class PersistenceContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Document> Documents { get; set; }

    public PersistenceContext(DbContextOptions<PersistenceContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) return;
        var config = new ConfigurationBuilder().Build();
        var builder = new NpgsqlDataSourceBuilder(config.GetConnectionString("MicroserviceDb"));
        builder.MapEnum<UserRole>("user_role");
        var datasource = builder.Build();
        optionsBuilder.UseNpgsql(datasource);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<UserRole>(schema: "public", name: "user_role");
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var changeDetection = new EntityState[] { EntityState.Added, EntityState.Modified, EntityState.Detached };
        var entries = ChangeTracker.Entries().Where(entry => Array.Exists(changeDetection, state => state == entry.State));
        var date = DateTime.UtcNow;
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Detached)
                entry.CurrentValues[nameof(EntityBase.DeletedAt)] = date;
            else
            {
                entry.CurrentValues[nameof(EntityBase.UpdatedAt)] = date;
                if (entry.State == EntityState.Added)
                    entry.CurrentValues[nameof(EntityBase.CreatedAt)] = date;
            }
        }
        return await base.SaveChangesAsync(cancellationToken);
    }
}

public static class RegisterDbContext
{
    public static IServiceCollection AddDataPersistence(this IServiceCollection services, string connectionString)
    {
        return services.AddDbContext<PersistenceContext>(option =>
        {
            var builder = new NpgsqlDataSourceBuilder(connectionString);
            builder.MapEnum<UserRole>("user_role");
            var datasource = builder.Build();
            option
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .UseNpgsql(datasource, npgsqlOptionsAction: npgsqlOptions =>
                {
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 10,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null);
                });
        });
    }
}
