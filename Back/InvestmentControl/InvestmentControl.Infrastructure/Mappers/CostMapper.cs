using InvestmentControl.Domain.Models;
using InvestmentControl.Infrastructure.Data.Entities;

namespace InvestmentControl.Infrastructure.Mappers;

public static class CostMapper
{
    public static Cost ToDomain(CostEntity entity)
    {
        return new Cost(
            entity.Id,
            entity.ProjectId,
            entity.Amount,
            entity.Description,
            entity.Responsible,
            entity.Date,
            entity.CreatedAt);
    }

    public static CostEntity ToEntity(Cost domain)
    {
        return new CostEntity
        {
            Id = domain.Id,
            ProjectId = domain.ProjectId,
            Amount = domain.Amount,
            Description = domain.Description,
            Responsible = domain.Responsible,
            Date = domain.Date,
            CreatedAt = domain.CreatedAt
        };
    }
}
