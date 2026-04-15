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
        if (progressPercentage < 1 || progressPercentage > 100)
            throw new ArgumentException("Прогресс должен быть в диапазоне от 1 до 100.", nameof(progressPercentage));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Описание не может быть пустым.", nameof(description));

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
        if (progressPercentage < 1 || progressPercentage > 100)
            throw new ArgumentException("Прогресс должен быть в диапазоне от 1 до 100.", nameof(progressPercentage));
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Описание не может быть пустым.", nameof(description));

        Description = description;
        ProgressPercentage = progressPercentage;
        UpdatedAt = DateTime.UtcNow;
    }
}