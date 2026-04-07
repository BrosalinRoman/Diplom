using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InvestmentControl.Domain.ReadModels;

namespace InvestmentControl.Infrastructure.Data.Configurations;

public class StatusReadModelConfiguration : IEntityTypeConfiguration<StatusReadModel>
{
    public void Configure(EntityTypeBuilder<StatusReadModel> builder)
    {
        builder.ToTable("statuses", "nsi_service");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("id");
        builder.Property(s => s.Name).HasColumnName("name");
        builder.Property(s => s.Description).HasColumnName("description");
    }
}