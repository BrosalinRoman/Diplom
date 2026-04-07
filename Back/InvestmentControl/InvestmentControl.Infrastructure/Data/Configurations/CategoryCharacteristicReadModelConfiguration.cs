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

        // Внешние ключи
        builder.HasOne(cc => cc.Category)
               .WithMany()
               .HasForeignKey(cc => cc.CategoryId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_category_characteristics_category_id");

        builder.HasOne(cc => cc.Characteristic)
               .WithMany()
               .HasForeignKey(cc => cc.CharacteristicId)
               .OnDelete(DeleteBehavior.Restrict)
               .HasConstraintName("fk_category_characteristics_characteristic_id");
    }
}