namespace InvestmentControl.Application.Analytics.DTOs;

public class DepartmentSummaryDto
{
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public int ProjectCount { get; set; }
    public decimal TotalBudget { get; set; }
}
