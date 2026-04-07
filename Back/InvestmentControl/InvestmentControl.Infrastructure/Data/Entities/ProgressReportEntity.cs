namespace InvestmentControl.Infrastructure.Data.Entities;

public class ProgressReportEntity
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public string Description { get; set; } = string.Empty;

    public decimal ProgressPercentage { get; set; }

    public DateTime ReportDate { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
