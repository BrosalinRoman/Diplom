using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InvestmentControl.Domain.ReadModels;

namespace InvestmentControl.Infrastructure.Data.Configurations;

public class DepartmentReadModelConfiguration : IEntityTypeConfiguration<DepartmentReadModel>
{
    public void Configure(EntityTypeBuilder<DepartmentReadModel> builder)
    {
        builder.ToTable("departments", "user_service");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).HasColumnName("id");
        builder.Property(d => d.Name).HasColumnName("name");
        builder.Property(d => d.Description).HasColumnName("description");
    }
}