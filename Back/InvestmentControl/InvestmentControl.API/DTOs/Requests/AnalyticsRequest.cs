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
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
