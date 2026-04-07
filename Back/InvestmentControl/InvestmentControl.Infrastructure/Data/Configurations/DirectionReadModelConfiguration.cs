using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InvestmentControl.Domain.ReadModels;

namespace InvestmentControl.Infrastructure.Data.Configurations;

public class DirectionReadModelConfiguration : IEntityTypeConfiguration<DirectionReadModel>
{
    public void Configure(EntityTypeBuilder<DirectionReadModel> builder)
    {
        builder.ToTable("directions", "nsi_service");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).HasColumnName("id");
        builder.Property(d => d.Name).HasColumnName("name");
        builder.Property(d => d.Description).HasColumnName("description");
    }
}