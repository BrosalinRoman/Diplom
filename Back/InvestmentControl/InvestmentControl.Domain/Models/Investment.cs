namespace InvestmentControl.Domain.Models;

/// <summary>
/// Доменная модель инвестиции (плановая и фактическая).
/// </summary>
public class Investment
{
    public int Id { get; private set; }
    public int ProjectId { get; private set; }
    public decimal? PlannedAmount { get; private set; }
    public DateTime? PlannedDate { get; private set; }
    public decimal? ActualAmount { get; private set; }
    public DateTime? ActualDate { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Для создания новой инвестиции (например, при добавлении фактической)
    public Investment(int projectId, decimal? plannedAmount, DateTime? plannedDate, decimal? actualAmount, DateTime? actualDate)
    {
        ProjectId = projectId;
        PlannedAmount = plannedAmount;
        PlannedDate = plannedDate;
        ActualAmount = actualAmount;
        ActualDate = actualDate;
        CreatedAt = DateTime.UtcNow;

        // Инвариант: хотя бы одно из полей должно быть заполнено
        if (!PlannedAmount.HasValue && !ActualAmount.HasValue)
            throw new ArgumentException("Должна быть указана либо плановая, либо фактическая сумма.");
    }

    // Конструктор для восстановления из БД
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

    // Обновление инвестиции (например, инвестор корректирует)
    public void Update(decimal? plannedAmount, DateTime? plannedDate, decimal? actualAmount, DateTime? actualDate)
    {
        PlannedAmount = plannedAmount;
        PlannedDate = plannedDate;
        ActualAmount = actualAmount;
        ActualDate = actualDate;
        if (!PlannedAmount.HasValue && !ActualAmount.HasValue)
            throw new ArgumentException("Должна быть указана либо плановая, либо фактическая сумма.");
    }
}
