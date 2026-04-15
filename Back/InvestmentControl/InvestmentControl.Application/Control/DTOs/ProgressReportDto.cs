namespace InvestmentControl.Application.Control.DTOs;

public class ProgressReportDto
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal ProgressPercentage { get; set; }
    public DateTime ReportDate { get; set; }
}