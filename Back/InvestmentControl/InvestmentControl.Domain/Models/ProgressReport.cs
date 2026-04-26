namespace InvestmentControl.Domain.Models;

public class ProgressReport
{
    public int Id { get; private set; }
    public int ProjectId { get; private set; }
    public string Description { get; private set; }
    public decimal ProgressPercentage { get; private set; }
    public DateTime ReportDate { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public ProgressReport(int projectId, string description, decimal progressPercentage)
    {
        if (projectId <= 0)
            throw new ArgumentException("ID проекта должен быть положительным.");

        ValidateProgress(progressPercentage);
        ValidateDescription(description);

        ProjectId = projectId;
        Description = description;
        ProgressPercentage = progressPercentage;
        ReportDate = DateTime.UtcNow;
        UpdatedAt = null;
    }

    public ProgressReport(int id, int projectId, string description, decimal progressPercentage, DateTime reportDate, DateTime? updatedAt)
    {
        Id = id;
        ProjectId = projectId;
        Description = description;
        ProgressPercentage = progressPercentage;
        ReportDate = reportDate;
        UpdatedAt = updatedAt;
    }

    public void Update(string description, decimal progressPercentage)
    {
        ValidateProgress(progressPercentage);
        ValidateDescription(description);

        Description = description;
        ProgressPercentage = progressPercentage;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateProgress(decimal progress)
    {
        if (progress < 1 || progress > 100)
            throw new ArgumentException("Прогресс должен быть в диапазоне от 1 до 100.", nameof(progress));
    }

    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Описание не может быть пустым.", nameof(description));
        if (description.Length > 500)
            throw new ArgumentException("Описание не должно превышать 500 символов.", nameof(description));
    }
}