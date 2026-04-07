namespace InvestmentControl.API.DTOs.Requests;

public class ProgressReportRequest
{
    public string Description { get; set; } = string.Empty;
    public decimal ProgressPercentage { get; set; }
}
