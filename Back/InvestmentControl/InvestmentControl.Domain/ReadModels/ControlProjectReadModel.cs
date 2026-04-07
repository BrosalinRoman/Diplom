namespace InvestmentControl.Domain.ReadModels;

public class ControlProjectReadModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public decimal Invested { get; set; }
    public decimal Progress { get; set; }
    public DateTime StartDate { get; set; }
}