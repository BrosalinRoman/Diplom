namespace InvestmentControl.Application.Analytics.DTOs;

public class TemplateDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string FiltersJson { get; set; } = string.Empty;
}