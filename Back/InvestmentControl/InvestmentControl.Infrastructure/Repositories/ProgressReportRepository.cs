using InvestmentControl.Domain.Interfaces;
using InvestmentControl.Domain.Models;
using InvestmentControl.Infrastructure.Data;
using InvestmentControl.Infrastructure.Data.Entities;
using InvestmentControl.Infrastructure.Mappers;
using InvestmentControl.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

public class ProgressReportRepository : GenericRepository<ProgressReportEntity, ProgressReport, ControlDbContext>, IProgressReportRepository
{
    public ProgressReportRepository(ControlDbContext context) : base(context) { }

    public async Task<IEnumerable<ProgressReport>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default)
    {
        var entities = await _dbSet.Where(r => r.ProjectId == projectId).ToListAsync(cancellationToken);
        return entities.Select(ProgressReportMapper.ToDomain);
    }

    protected override ProgressReport MapToDomain(ProgressReportEntity entity) => ProgressReportMapper.ToDomain(entity);
    protected override ProgressReportEntity MapToEntity(ProgressReport domain) => ProgressReportMapper.ToEntity(domain);
}
