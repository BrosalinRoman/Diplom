namespace InvestmentControl.Infrastructure.Data.Entities;

public class CostEntity
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public decimal Amount { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Responsible { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public DateTime CreatedAt { get; set; }
}
