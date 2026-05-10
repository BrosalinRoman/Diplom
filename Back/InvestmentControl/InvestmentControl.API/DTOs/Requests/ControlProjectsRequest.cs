namespace InvestmentControl.API.DTOs.Requests;

public class ControlProjectsRequest
{
    public string? Search { get; set; }
    public List<int>? DirectionIds { get; set; }
    public List<int>? DepartmentIds { get; set; }
    public List<int>? CategoryIds { get; set; }
    public List<int>? StatusIds { get; set; }   // Новый фильтр
    public string? Sort { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}