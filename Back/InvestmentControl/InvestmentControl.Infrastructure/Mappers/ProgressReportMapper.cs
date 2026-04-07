using InvestmentControl.Domain.Models;
using InvestmentControl.Infrastructure.Data.Entities;

namespace InvestmentControl.Infrastructure.Mappers;

public static class ProgressReportMapper
{
    public static ProgressReport ToDomain(ProgressReportEntity entity)
    {
        return new ProgressReport(
            entity.Id,
            entity.ProjectId,
            entity.Description,
            entity.ProgressPercentage,
            entity.ReportDate,
            entity.UpdatedAt);
    }

    public static ProgressReportEntity ToEntity(ProgressReport domain)
    {
        return new ProgressReportEntity
        {
            Id = domain.Id,
            ProjectId = domain.ProjectId,
            Description = domain.Description,
            ProgressPercentage = domain.ProgressPercentage,
            ReportDate = domain.ReportDate,
            UpdatedAt = domain.UpdatedAt
        };
    }
}
