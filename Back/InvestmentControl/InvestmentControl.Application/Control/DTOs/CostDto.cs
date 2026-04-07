namespace InvestmentControl.Application.Control.DTOs;

public class CostDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Responsible { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}
