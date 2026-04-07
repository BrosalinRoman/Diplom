using InvestmentControl.Application.Analytics.DTOs;

namespace InvestmentControl.API.DTOs.Responses;

public class SummaryResponse
{
    public List<DepartmentSummaryDto> Departments { get; set; } = new();
    public decimal TotalBudget { get; set; }
    public int TotalProjects { get; set; }
}