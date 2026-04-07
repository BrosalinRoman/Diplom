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
    }
}