namespace InvestmentControl.Domain.ReadModels;

public class StatusReadModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}