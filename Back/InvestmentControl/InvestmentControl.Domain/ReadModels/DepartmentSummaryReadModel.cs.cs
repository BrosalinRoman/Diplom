namespace InvestmentControl.Domain.ReadModels;

public class DepartmentSummaryReadModel
{
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public int ProjectCount { get; set; }
    public decimal TotalBudget { get; set; }
}