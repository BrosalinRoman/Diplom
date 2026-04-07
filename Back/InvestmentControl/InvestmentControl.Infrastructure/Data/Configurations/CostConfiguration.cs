using InvestmentControl.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvestmentControl.Infrastructure.Data.Configurations;

public class CostConfiguration : IEntityTypeConfiguration<CostEntity>
{
    public void Configure(EntityTypeBuilder<CostEntity> builder)
    {
        builder.ToTable("costs");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");
        builder.Property(c => c.ProjectId).HasColumnName("project_id").IsRequired();
        builder.Property(c => c.Amount).HasColumnName("amount").HasColumnType("numeric");
        builder.Property(c => c.Description).HasColumnName("description");
        builder.Property(c => c.Responsible).HasColumnName("responsible");
        builder.Property(c => c.Date).HasColumnName("date");
        builder.Property(c => c.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(c => c.ProjectId).HasDatabaseName("ix_costs_project_id");

        // Внешний ключ – но так как ProjectEntity нет в этом контексте, только если добавить DbSet<ProjectEntity>
        // Поэтому комментируем. В БД он есть, но EF не проверяет.
        // builder.HasOne<ProjectEntity>().WithMany().HasForeignKey(c => c.ProjectId).OnDelete(DeleteBehavior.Restrict);
    }
}
