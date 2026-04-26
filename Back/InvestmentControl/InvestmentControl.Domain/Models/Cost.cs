using System.Globalization;
using System.Text.RegularExpressions;

namespace InvestmentControl.Domain.Models;

public class Cost
{
    public int Id { get; private set; }
    public int ProjectId { get; private set; }
    public decimal Amount { get; private set; }
    public string Description { get; private set; }
    public string Responsible { get; private set; }
    public DateTime Date { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Cost(int projectId, decimal amount, string description, string responsible, DateTime date)
    {
        if (projectId <= 0)
            throw new ArgumentException("ID проекта должен быть положительным.");

        ValidateAmount(amount);
        ValidateDescription(description);
        ValidateResponsible(responsible);

        ProjectId = projectId;
        Amount = amount;
        Description = description;
        Responsible = responsible;
        Date = date;
        CreatedAt = DateTime.UtcNow;
    }

    public Cost(int id, int projectId, decimal amount, string description, string responsible, DateTime date, DateTime createdAt)
    {
        Id = id;
        ProjectId = projectId;
        Amount = amount;
        Description = description;
        Responsible = responsible;
        Date = date;
        CreatedAt = createdAt;
    }

    public void Update(decimal amount, string description, string responsible, DateTime date)
    {
        ValidateAmount(amount);
        ValidateDescription(description);
        ValidateResponsible(responsible);

        Amount = amount;
        Description = description;
        Responsible = responsible;
        Date = date;
    }

    private static void ValidateAmount(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Сумма затрат должна быть положительной.", nameof(amount));

        // Максимум 17 знаков до запятой + не более 2 после
        var parts = amount.ToString(CultureInfo.InvariantCulture).Split('.');
        if (parts[0].Length > 17)
            throw new ArgumentException("Сумма затрат не может превышать 17 цифр до запятой.", nameof(amount));
        if (parts.Length == 2 && parts[1].Length > 2)
            throw new ArgumentException("Сумма затрат может содержать не более 2 знаков после запятой.", nameof(amount));
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Описание не может быть пустым.", nameof(description));
        if (description.Length > 500)
            throw new ArgumentException("Описание не должно превышать 500 символов.", nameof(description));
    }

    private static void ValidateResponsible(string responsible)
    {
        if (string.IsNullOrWhiteSpace(responsible))
            throw new ArgumentException("Ответственный не может быть пустым.", nameof(responsible));
        if (responsible.Length > 50)
            throw new ArgumentException("Ответственный не должен превышать 50 символов.", nameof(responsible));
        // Формат: Фамилия И.О. (например, "Иванов И.И.")
        var regex = new Regex(@"^[А-Яа-яёЁA-Za-z\-]{1,50}\s[А-Яа-яёЁA-Za-z]\.[А-Яа-яёЁA-Za-z]\.$");
        if (!regex.IsMatch(responsible))
            throw new ArgumentException("Ответственный должен быть в формате 'Фамилия И.О.' (например, Иванов И.И.).", nameof(responsible));
    }
}