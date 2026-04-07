using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InvestmentControl.Domain.ReadModels;

namespace InvestmentControl.Infrastructure.Data.Configurations;

public class CategoryCharacteristicReadModelConfiguration : IEntityTypeConfiguration<CategoryCharacteristicReadModel>
{
    public void Configure(EntityTypeBuilder<CategoryCharacteristicReadModel> builder)
    {
        builder.ToTable("category_characteristics", "nsi_service");
        builder.HasKey(cc => cc.Id);
        builder.Property(cc => cc.Id).HasColumnName("id");
        builder.Property(cc => cc.CategoryId).HasColumnName("category_id");
        builder.Property(cc => cc.CharacteristicId).HasColumnName("characteristic_id");
    }
}