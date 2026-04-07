using InvestmentControl.Domain.Models;

namespace InvestmentControl.Domain.Interfaces;

public interface ITemplateRepository
{
    Task<Template?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Template>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task AddAsync(Template template, CancellationToken cancellationToken = default);
    void Update(Template template);
    void Delete(Template template);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
