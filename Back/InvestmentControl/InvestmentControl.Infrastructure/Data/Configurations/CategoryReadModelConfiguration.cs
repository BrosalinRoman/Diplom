using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InvestmentControl.Domain.ReadModels;

namespace InvestmentControl.Infrastructure.Data.Configurations;

public class CategoryReadModelConfiguration : IEntityTypeConfiguration<CategoryReadModel>
{
    public void Configure(EntityTypeBuilder<CategoryReadModel> builder)
    {
        builder.ToTable("categories", "nsi_service");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");
        builder.Property(c => c.Name).HasColumnName("name");
        builder.Property(c => c.Description).HasColumnName("description");
    }
}