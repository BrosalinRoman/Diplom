namespace InvestmentControl.API.DTOs.Requests;

public class SummaryRequest
{
    public List<int>? DepartmentIds { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public List<int>? StatusIds { get; set; }
    public List<int>? DirectionIds { get; set; }
    public List<int>? CategoryIds { get; set; }
    public int? BudgetFieldId { get; set; }
}
