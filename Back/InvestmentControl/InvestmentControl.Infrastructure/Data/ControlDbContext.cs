// Infrastructure/Data/ControlDbContext.cs
using Microsoft.EntityFrameworkCore;
using InvestmentControl.Infrastructure.Data.Entities;

namespace InvestmentControl.Infrastructure.Data;

public class ControlDbContext : DbContext
{
    public ControlDbContext(DbContextOptions<ControlDbContext> options) : base(options) { }

    public DbSet<InvestmentEntity> Investments { get; set; }
    public DbSet<CostEntity> Costs { get; set; }
    public DbSet<ProgressReportEntity> ProgressReports { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("control_service");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ControlDbContext).Assembly);
    }
}
