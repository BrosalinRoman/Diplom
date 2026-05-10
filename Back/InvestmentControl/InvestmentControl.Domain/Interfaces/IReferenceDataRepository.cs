using InvestmentControl.Domain.ReadModels;

namespace InvestmentControl.Domain.Interfaces;

public interface IReferenceDataRepository
{
    Task<List<StatusReadModel>> GetStatusesAsync(CancellationToken cancellationToken);
    Task<List<CategoryReadModel>> GetCategoriesAsync(CancellationToken cancellationToken);
    Task<List<DirectionReadModel>> GetDirectionsAsync(CancellationToken cancellationToken);
    Task<List<DepartmentReadModel>> GetDepartmentsAsync(CancellationToken cancellationToken);
}