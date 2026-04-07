using Microsoft.EntityFrameworkCore;
using InvestmentControl.Domain.ReadModels;

namespace InvestmentControl.Infrastructure.Data;

public class ReadOnlyAppDbContext : DbContext
{
    public ReadOnlyAppDbContext(DbContextOptions<ReadOnlyAppDbContext> options) : base(options)
    {
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
    }

    public DbSet<ProjectReadModel> Projects { get; set; }
    public DbSet<ProjectCharacteristicValueReadModel> ProjectCharacteristicValues { get; set; }
    public DbSet<CategoryReadModel> Categories { get; set; }
    public DbSet<DirectionReadModel> Directions { get; set; }
    public DbSet<StatusReadModel> Statuses { get; set; }
    public DbSet<DepartmentReadModel> Departments { get; set; }
    public DbSet<CharacteristicReadModel> Characteristics { get; set; }
    public DbSet<CategoryCharacteristicReadModel> CategoryCharacteristics { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReadOnlyAppDbContext).Assembly);
    }
}