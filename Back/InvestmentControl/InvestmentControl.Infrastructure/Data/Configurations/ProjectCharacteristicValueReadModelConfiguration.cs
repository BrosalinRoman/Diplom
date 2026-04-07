using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InvestmentControl.Domain.ReadModels;

namespace InvestmentControl.Infrastructure.Data.Configurations;

public class ProjectCharacteristicValueReadModelConfiguration : IEntityTypeConfiguration<ProjectCharacteristicValueReadModel>
{
    public void Configure(EntityTypeBuilder<ProjectCharacteristicValueReadModel> builder)
    {
        builder.ToTable("project_characteristic_values", "project_service");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id");
        builder.Property(p => p.ProjectId).HasColumnName("project_id");
        builder.Property(p => p.CategoryCharacteristicId).HasColumnName("category_characteristic_id");
        builder.Property(p => p.Value).HasColumnName("value");
        builder.Property(p => p.Score).HasColumnName("score");
    }
}