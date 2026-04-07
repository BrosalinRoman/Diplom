using InvestmentControl.Domain.Models;

namespace InvestmentControl.Domain.Interfaces;

public interface IProgressReportRepository
{
    Task<ProgressReport?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProgressReport>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
    Task AddAsync(ProgressReport report, CancellationToken cancellationToken = default);
    void Update(ProgressReport report);
    void Delete(ProgressReport report);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
