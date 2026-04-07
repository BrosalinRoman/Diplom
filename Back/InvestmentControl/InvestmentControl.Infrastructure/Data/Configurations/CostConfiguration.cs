using InvestmentControl.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
        builder.HasIndex(c => c.ProjectId);
    }
}
