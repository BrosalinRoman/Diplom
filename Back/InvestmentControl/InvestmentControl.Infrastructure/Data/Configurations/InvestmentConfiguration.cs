// Infrastructure/Data/Configurations/InvestmentConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InvestmentControl.Infrastructure.Data.Entities;

namespace InvestmentControl.Infrastructure.Data.Configurations;

public class InvestmentConfiguration : IEntityTypeConfiguration<InvestmentEntity>
{
    public void Configure(EntityTypeBuilder<InvestmentEntity> builder)
    {
        builder.ToTable("investments");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).HasColumnName("id");
        builder.Property(i => i.ProjectId).HasColumnName("project_id").IsRequired();
        builder.Property(i => i.PlannedAmount).HasColumnName("planned_amount").HasColumnType("numeric");
        builder.Property(i => i.PlannedDate).HasColumnName("planned_date");
        builder.Property(i => i.ActualAmount).HasColumnName("actual_amount").HasColumnType("numeric");
        builder.Property(i => i.ActualDate).HasColumnName("actual_date");
        builder.Property(i => i.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Можно добавить индекс
        builder.HasIndex(i => i.ProjectId);
    }
}
