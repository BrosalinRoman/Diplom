namespace InvestmentControl.Application.Analytics.DTOs;

public class ProjectAnalyticsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal? Rank { get; set; }
    public Dictionary<string, decimal?> Characteristics { get; set; } = new();

    // Новые поля
    public string Category { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
}
