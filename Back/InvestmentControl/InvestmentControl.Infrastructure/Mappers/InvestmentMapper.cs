using InvestmentControl.Domain.Models;
using InvestmentControl.Infrastructure.Data.Entities;

namespace InvestmentControl.Infrastructure.Mappers;

public static class InvestmentMapper
{
    public static Investment ToDomain(InvestmentEntity entity)
    {
        return new Investment(
            entity.Id,
            entity.ProjectId,
            entity.PlannedAmount,
            entity.PlannedDate,
            entity.ActualAmount,
            entity.ActualDate,
            entity.CreatedAt);
    }

    public static InvestmentEntity ToEntity(Investment domain)
    {
        return new InvestmentEntity
        {
            Id = domain.Id,
            ProjectId = domain.ProjectId,
            PlannedAmount = domain.PlannedAmount,
            PlannedDate = domain.PlannedDate,
            ActualAmount = domain.ActualAmount,
            ActualDate = domain.ActualDate,
            CreatedAt = domain.CreatedAt
        };
    }
}
