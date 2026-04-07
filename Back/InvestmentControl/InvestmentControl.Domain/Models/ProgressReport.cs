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
        Description = description;
        ProgressPercentage = progressPercentage;
        UpdatedAt = DateTime.UtcNow;
    }
}
