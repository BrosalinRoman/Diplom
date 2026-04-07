namespace InvestmentControl.Application.Control.DTOs;

public class InvestmentDto
{
    public int Id { get; set; }
    public decimal? PlannedAmount { get; set; }
    public DateTime? PlannedDate { get; set; }
    public decimal? ActualAmount { get; set; }
    public DateTime? ActualDate { get; set; }
}
