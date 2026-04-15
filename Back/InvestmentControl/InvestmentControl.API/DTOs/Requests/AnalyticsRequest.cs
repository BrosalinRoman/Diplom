namespace InvestmentControl.API.DTOs.Requests;

public class AnalyticsRequest
{
    public int CategoryId { get; set; }
    public List<int>? DirectionIds { get; set; }
    public List<int>? DepartmentIds { get; set; }
    public List<int>? StatusIds { get; set; }
    public decimal? RankMin { get; set; }
    public decimal? RankMax { get; set; }
    public List<int>? ProjectIds { get; set; }
    public List<string>? SelectedFields { get; set; }
    public string? Search { get; set; }
}
