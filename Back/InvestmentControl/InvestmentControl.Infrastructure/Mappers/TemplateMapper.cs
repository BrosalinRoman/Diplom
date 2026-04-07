using InvestmentControl.Domain.Models;
using InvestmentControl.Infrastructure.Data.Entities;

namespace InvestmentControl.Infrastructure.Mappers;

public static class TemplateMapper
{
    public static Template ToDomain(TemplateEntity entity)
    {
        return new Template(
            entity.Id,
            entity.Name,
            entity.UserId,
            entity.FiltersJson,
            entity.CreatedAt,
            entity.UpdatedAt);
    }

    public static TemplateEntity ToEntity(Template domain)
    {
        return new TemplateEntity
        {
            Id = domain.Id,
            Name = domain.Name,
            UserId = domain.UserId,
            FiltersJson = domain.FiltersJson,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }
}
