using InvestmentControl.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class TemplateConfiguration : IEntityTypeConfiguration<TemplateEntity>
{
    public void Configure(EntityTypeBuilder<TemplateEntity> builder)
    {
        builder.ToTable("templates", "analytics_service");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("id");
        builder.Property(t => t.Name).HasColumnName("name").IsRequired();
        builder.Property(t => t.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(t => t.FiltersJson).HasColumnName("filters").HasColumnType("jsonb").IsRequired();
        builder.Property(t => t.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(t => t.UpdatedAt).HasColumnName("updated_at");
        builder.HasIndex(t => t.UserId);
    }
}
