namespace InvestmentControl.Infrastructure.Data.Entities;

public class InvestmentEntity
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public decimal? PlannedAmount { get; set; }

    public DateTime? PlannedDate { get; set; }

    public decimal? ActualAmount { get; set; }

    public DateTime? ActualDate { get; set; }

    public DateTime CreatedAt { get; set; }
}
