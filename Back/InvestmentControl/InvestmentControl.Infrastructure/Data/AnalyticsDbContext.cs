// Infrastructure/Data/AnalyticsDbContext.cs
using Microsoft.EntityFrameworkCore;
using InvestmentControl.Infrastructure.Data.Entities;

namespace InvestmentControl.Infrastructure.Data;

public class AnalyticsDbContext : DbContext
{
    public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : base(options) { }

    public DbSet<TemplateEntity> Templates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("analytics_service");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AnalyticsDbContext).Assembly);
    }
}
