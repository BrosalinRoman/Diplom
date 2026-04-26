using System.Globalization;

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

    // Конструктор для создания новой инвестиции
    public Investment(int projectId, decimal? plannedAmount, DateTime? plannedDate, decimal? actualAmount, DateTime? actualDate)
    {
        if (projectId <= 0)
            throw new ArgumentException("ID проекта должен быть положительным.", nameof(projectId));

        ValidateAmounts(plannedAmount, actualAmount);
        ValidateDates(plannedAmount, plannedDate, actualAmount, actualDate);
        ValidatePlannedVsActualDates(plannedDate, actualDate);

        ProjectId = projectId;
        PlannedAmount = plannedAmount;
        PlannedDate = plannedDate;
        ActualAmount = actualAmount;
        ActualDate = actualDate;
        CreatedAt = DateTime.UtcNow;
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

    // Обновление инвестиции
    public void Update(decimal? plannedAmount, DateTime? plannedDate, decimal? actualAmount, DateTime? actualDate)
    {
        ValidateAmounts(plannedAmount, actualAmount);
        ValidateDates(plannedAmount, plannedDate, actualAmount, actualDate);
        ValidatePlannedVsActualDates(plannedDate, actualDate);

        PlannedAmount = plannedAmount;
        PlannedDate = plannedDate;
        ActualAmount = actualAmount;
        ActualDate = actualDate;
    }

    private static void ValidateAmounts(decimal? plannedAmount, decimal? actualAmount)
    {
        if (!plannedAmount.HasValue && !actualAmount.HasValue)
            throw new ArgumentException("Должна быть указана либо плановая, либо фактическая сумма.");

        void ValidateSingle(decimal? amount, string name)
        {
            if (!amount.HasValue) return;
            if (amount.Value <= 0)
                throw new ArgumentException($"{name} должна быть положительной.", name);
            var parts = amount.Value.ToString(CultureInfo.InvariantCulture).Split('.');
            if (parts[0].Length > 17)
                throw new ArgumentException($"{name} не может превышать 17 цифр до запятой.", name);
            if (parts.Length == 2 && parts[1].Length > 2)
                throw new ArgumentException($"{name} может содержать не более 2 знаков после запятой.", name);
        }

        ValidateSingle(plannedAmount, "Плановая сумма");
        ValidateSingle(actualAmount, "Фактическая сумма");
    }

    private static void ValidateDates(decimal? plannedAmount, DateTime? plannedDate, decimal? actualAmount, DateTime? actualDate)
    {
        if (plannedAmount.HasValue && !plannedDate.HasValue)
            throw new ArgumentException("Для плановой суммы необходимо указать плановую дату.");
        if (plannedDate.HasValue && !plannedAmount.HasValue)
            throw new ArgumentException("Для плановой даты необходимо указать плановую сумму.");

        if (actualAmount.HasValue && !actualDate.HasValue)
            throw new ArgumentException("Для фактической суммы необходимо указать фактическую дату.");
        if (actualDate.HasValue && !actualAmount.HasValue)
            throw new ArgumentException("Для фактической даты необходимо указать фактическую сумму.");
    }

    private static void ValidatePlannedVsActualDates(DateTime? plannedDate, DateTime? actualDate)
    {
        if (plannedDate.HasValue && actualDate.HasValue && actualDate < plannedDate)
            throw new ArgumentException("Фактическая дата не может быть раньше плановой.");
    }
}