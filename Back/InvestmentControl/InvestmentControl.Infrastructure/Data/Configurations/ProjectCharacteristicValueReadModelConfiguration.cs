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

        // Внешние ключи
        builder.HasOne(pcv => pcv.Project)
               .WithMany()
               .HasForeignKey(pcv => pcv.ProjectId)
               .OnDelete(DeleteBehavior.Cascade)
               .HasConstraintName("fk_pcv_project_id");

        builder.HasOne(pcv => pcv.CategoryCharacteristic)
               .WithMany()
               .HasForeignKey(pcv => pcv.CategoryCharacteristicId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_pcv_category_characteristic_id");

        // Составной индекс для ускорения запросов
        builder.HasIndex(pcv => new { pcv.ProjectId, pcv.CategoryCharacteristicId })
               .HasDatabaseName("ix_pcv_project_characteristic");
    }
}