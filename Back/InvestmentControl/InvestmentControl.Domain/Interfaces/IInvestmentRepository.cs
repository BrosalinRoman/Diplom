using InvestmentControl.Domain.Models;

namespace InvestmentControl.Domain.Interfaces;

public interface IInvestmentRepository
{
    Task<Investment?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Investment>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
    Task AddAsync(Investment investment, CancellationToken cancellationToken = default);
    void Update(Investment investment);
    void Delete(Investment investment);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
