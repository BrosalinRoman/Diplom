using InvestmentControl.Domain.Interfaces;
using InvestmentControl.Domain.Models;
using InvestmentControl.Infrastructure.Data;
using InvestmentControl.Infrastructure.Data.Entities;
using InvestmentControl.Infrastructure.Mappers;
using InvestmentControl.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

public class InvestmentRepository : GenericRepository<InvestmentEntity, Investment, ControlDbContext>, IInvestmentRepository
{
    public InvestmentRepository(ControlDbContext context) : base(context) { }

    public async Task<IEnumerable<Investment>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default)
    {
        var entities = await _dbSet.Where(i => i.ProjectId == projectId).ToListAsync(cancellationToken);
        return entities.Select(InvestmentMapper.ToDomain);
    }

    protected override Investment MapToDomain(InvestmentEntity entity) => InvestmentMapper.ToDomain(entity);
    protected override InvestmentEntity MapToEntity(Investment domain) => InvestmentMapper.ToEntity(domain);
}
