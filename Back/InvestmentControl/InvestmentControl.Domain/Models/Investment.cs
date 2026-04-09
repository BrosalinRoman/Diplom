namespace InvestmentControl.Domain.Models;

public class Investment
{
    public int Id { get; private set; }
    public int ProjectId { get; private set; }
    public decimal? PlannedAmount { get; private set; }
    public DateTime? PlannedDate { get; private set; }
    public decimal? ActualAmount { get; private set; }
    public DateTime? ActualDate { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Investment(int projectId, decimal? plannedAmount, DateTime? plannedDate, decimal? actualAmount, DateTime? actualDate)
    {
        if (!plannedAmount.HasValue && !actualAmount.HasValue)
            throw new ArgumentException("Должна быть указана либо плановая, либо фактическая сумма.");
        if (plannedAmount.HasValue && plannedAmount <= 0)
            throw new ArgumentException("Плановая сумма должна быть положительной.", nameof(plannedAmount));
        if (actualAmount.HasValue && actualAmount <= 0)
            throw new ArgumentException("Фактическая сумма должна быть положительной.", nameof(actualAmount));

        ProjectId = projectId;
        PlannedAmount = plannedAmount;
        PlannedDate = plannedDate;
        ActualAmount = actualAmount;
        ActualDate = actualDate;
        CreatedAt = DateTime.UtcNow;
    }

    public Investment(int id, int projectId, decimal? plannedAmount, DateTime? plannedDate, decimal? actualAmount, DateTime? actualDate, DateTime createdAt)
    {
        Id = id;
        ProjectId = projectId;
        PlannedAmount = plannedAmount;
        PlannedDate = plannedDate;
        ActualAmount = actualAmount;
        ActualDate = actualDate;
        CreatedAt = createdAt;
    }

    public void Update(decimal? plannedAmount, DateTime? plannedDate, decimal? actualAmount, DateTime? actualDate)
    {
        if (!plannedAmount.HasValue && !actualAmount.HasValue)
            throw new ArgumentException("Должна быть указана либо плановая, либо фактическая сумма.");
        if (plannedAmount.HasValue && plannedAmount <= 0)
            throw new ArgumentException("Плановая сумма должна быть положительной.", nameof(plannedAmount));
        if (actualAmount.HasValue && actualAmount <= 0)
            throw new ArgumentException("Фактическая сумма должна быть положительной.", nameof(actualAmount));

        PlannedAmount = plannedAmount;
        PlannedDate = plannedDate;
        ActualAmount = actualAmount;
        ActualDate = actualDate;
    }
}