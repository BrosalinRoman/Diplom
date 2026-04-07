namespace InvestmentControl.API.DTOs.Requests;

public class CostRequest
{
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Responsible { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}
