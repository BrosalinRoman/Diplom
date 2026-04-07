using Microsoft.EntityFrameworkCore;
using InvestmentControl.Domain.Models;
using InvestmentControl.Domain.Interfaces;
using InvestmentControl.Infrastructure.Data;
using InvestmentControl.Infrastructure.Data.Entities;
using InvestmentControl.Infrastructure.Mappers;

namespace InvestmentControl.Infrastructure.Repositories;

public class TemplateRepository : GenericRepository<TemplateEntity, Template, AnalyticsDbContext>, ITemplateRepository
{
    public TemplateRepository(AnalyticsDbContext context) : base(context) { }

    public async Task<IEnumerable<Template>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var entities = await _dbSet.Where(t => t.UserId == userId).ToListAsync(cancellationToken);
        return entities.Select(TemplateMapper.ToDomain);
    }

    protected override Template MapToDomain(TemplateEntity entity) => TemplateMapper.ToDomain(entity);
    protected override TemplateEntity MapToEntity(Template domain) => TemplateMapper.ToEntity(domain);
}
