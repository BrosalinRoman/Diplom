using InvestmentControl.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProgressReportConfiguration : IEntityTypeConfiguration<ProgressReportEntity>
{
    public void Configure(EntityTypeBuilder<ProgressReportEntity> builder)
    {
        builder.ToTable("progress_reports");
        builder.HasKey(pr => pr.Id);
        builder.Property(pr => pr.Id).HasColumnName("id");
        builder.Property(pr => pr.ProjectId).HasColumnName("project_id").IsRequired();
        builder.Property(pr => pr.Description).HasColumnName("description");
        builder.Property(pr => pr.ProgressPercentage).HasColumnName("progress_percentage").HasColumnType("numeric");
        builder.Property(pr => pr.ReportDate).HasColumnName("report_date").HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(pr => pr.UpdatedAt).HasColumnName("updated_at");
        builder.HasIndex(pr => pr.ProjectId);
    }
}
