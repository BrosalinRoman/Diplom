using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InvestmentControl.Domain.ReadModels;

namespace InvestmentControl.Infrastructure.Data.Configurations;

public class ProjectReadModelConfiguration : IEntityTypeConfiguration<ProjectReadModel>
{
    public void Configure(EntityTypeBuilder<ProjectReadModel> builder)
    {
        builder.ToTable("projects", "project_service");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id");
        builder.Property(p => p.Name).HasColumnName("name");
        builder.Property(p => p.Goal).HasColumnName("goal");
        builder.Property(p => p.CategoryId).HasColumnName("category_id");
        builder.Property(p => p.DirectionId).HasColumnName("direction_id");
        builder.Property(p => p.DepartmentId).HasColumnName("department_id");
        builder.Property(p => p.StatusId).HasColumnName("status_id");
        builder.Property(p => p.CreatedByUserId).HasColumnName("created_by_user_id");
        builder.Property(p => p.ResponsibleUserId).HasColumnName("responsible_user_id");
        builder.Property(p => p.Rank).HasColumnName("rank");
        builder.Property(p => p.Budget).HasColumnName("budget");
        builder.Property(p => p.CreatedAt).HasColumnName("created_at");
        builder.Property(p => p.PublishedAt).HasColumnName("published_at");

        // Игнорируем вычисляемое свойство
        builder.Ignore(p => p.Characteristics);

        // Внешние ключи (навигации)
        builder.HasOne(p => p.Category)
               .WithMany()
               .HasForeignKey(p => p.CategoryId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_projects_category_id");

        builder.HasOne(p => p.Direction)
               .WithMany()
               .HasForeignKey(p => p.DirectionId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_projects_direction_id");

        builder.HasOne(p => p.Department)
               .WithMany()
               .HasForeignKey(p => p.DepartmentId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_projects_department_id");

        builder.HasOne(p => p.Status)
               .WithMany()
               .HasForeignKey(p => p.StatusId)
               .OnDelete(DeleteBehavior.SetNull)
               .HasConstraintName("fk_projects_status_id");

        // Индексы
        builder.HasIndex(p => p.CategoryId).HasDatabaseName("ix_projects_category_id");
        builder.HasIndex(p => p.DirectionId).HasDatabaseName("ix_projects_direction_id");
        builder.HasIndex(p => p.DepartmentId).HasDatabaseName("ix_projects_department_id");
        builder.HasIndex(p => p.StatusId).HasDatabaseName("ix_projects_status_id");
    }
}