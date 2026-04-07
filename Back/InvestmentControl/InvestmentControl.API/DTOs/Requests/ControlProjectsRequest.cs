namespace InvestmentControl.API.DTOs.Requests;

public class ControlProjectsRequest
{
    public string? Search { get; set; }
    public List<int>? DirectionIds { get; set; }
    public List<int>? DepartmentIds { get; set; }
    public List<int>? CategoryIds { get; set; }
    public string? Sort { get; set; }
}
