namespace InvestmentControl.API.DTOs.Requests;

public class InvestmentRequest
{
    public decimal? PlannedAmount { get; set; }
    public DateTime? PlannedDate { get; set; }
    public decimal? ActualAmount { get; set; }
    public DateTime? ActualDate { get; set; }
}
