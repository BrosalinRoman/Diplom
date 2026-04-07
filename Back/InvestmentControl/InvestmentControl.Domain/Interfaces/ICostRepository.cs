using InvestmentControl.Domain.Models;

namespace InvestmentControl.Domain.Interfaces;

public interface ICostRepository
{
    Task<Cost?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Cost>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
    Task AddAsync(Cost cost, CancellationToken cancellationToken = default);
    void Update(Cost cost);
    void Delete(Cost cost);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
