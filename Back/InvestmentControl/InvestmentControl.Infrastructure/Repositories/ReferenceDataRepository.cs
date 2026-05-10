using Microsoft.EntityFrameworkCore;
using InvestmentControl.Domain.Interfaces;
using InvestmentControl.Domain.ReadModels;
using InvestmentControl.Infrastructure.Data;

namespace InvestmentControl.Infrastructure.Repositories;

public class ReferenceDataRepository : IReferenceDataRepository
{
    private readonly ReadOnlyAppDbContext _context;
    public ReferenceDataRepository(ReadOnlyAppDbContext context) => _context = context;

    public async Task<List<StatusReadModel>> GetStatusesAsync(CancellationToken cancellationToken)
        => await _context.Statuses.ToListAsync(cancellationToken);

    public async Task<List<CategoryReadModel>> GetCategoriesAsync(CancellationToken cancellationToken)
        => await _context.Categories.ToListAsync(cancellationToken);

    public async Task<List<DirectionReadModel>> GetDirectionsAsync(CancellationToken cancellationToken)
        => await _context.Directions.ToListAsync(cancellationToken);

    public async Task<List<DepartmentReadModel>> GetDepartmentsAsync(CancellationToken cancellationToken)
        => await _context.Departments.ToListAsync(cancellationToken);
}
