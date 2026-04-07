using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InvestmentControl.Domain.ReadModels;

namespace InvestmentControl.Infrastructure.Data.Configurations;

public class CharacteristicReadModelConfiguration : IEntityTypeConfiguration<CharacteristicReadModel>
{
    public void Configure(EntityTypeBuilder<CharacteristicReadModel> builder)
    {
        builder.ToTable("characteristics", "nsi_service");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("id");
        builder.Property(c => c.Name).HasColumnName("name");
        builder.Property(c => c.Unit).HasColumnName("unit");
    }
}