using Microsoft.EntityFrameworkCore;
using InvestmentControl.Domain.Interfaces;
using InvestmentControl.Domain.Models;
using InvestmentControl.Infrastructure.Data;
using InvestmentControl.Infrastructure.Data.Entities;
using InvestmentControl.Infrastructure.Mappers;

namespace InvestmentControl.Infrastructure.Repositories;

public class CostRepository : GenericRepository<CostEntity, Cost, ControlDbContext>, ICostRepository
{
    public CostRepository(ControlDbContext context) : base(context) { }

    public async Task<IEnumerable<Cost>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default)
    {
        var entities = await _dbSet.Where(c => c.ProjectId == projectId).ToListAsync(cancellationToken);
        return entities.Select(CostMapper.ToDomain);
    }

    protected override Cost MapToDomain(CostEntity entity) => CostMapper.ToDomain(entity);
    protected override CostEntity MapToEntity(Cost domain) => CostMapper.ToEntity(domain);
}