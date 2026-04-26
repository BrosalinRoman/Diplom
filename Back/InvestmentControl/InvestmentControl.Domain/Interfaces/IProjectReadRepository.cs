using InvestmentControl.Domain.ReadModels;

namespace InvestmentControl.Domain.Interfaces;

public interface IProjectReadRepository
{
    Task<List<ProjectReadModel>> GetFilteredProjectsAsync(
        int categoryId,
        List<int>? directionIds,
        List<int>? departmentIds,
        List<int>? statusIds,
        decimal? rankMin,
        decimal? rankMax,
        List<int>? allowedProjectIds,
        List<int>? excludedProjectIds,
        CancellationToken cancellationToken);

    Task<List<int>> GetProjectIdsByCreatorAsync(int userId, CancellationToken cancellationToken);

    Task<List<DepartmentSummaryReadModel>> GetDepartmentSummaryAsync(
        List<int>? departmentIds,
        DateTime? dateFrom,
        DateTime? dateTo,
        List<int>? statusIds,
        List<int>? directionIds,
        List<int>? categoryIds,
        CancellationToken cancellationToken);

    Task<List<ControlProjectReadModel>> GetControlProjectsAsync(
        string? search,
        List<int>? directionIds,
        List<int>? departmentIds,
        List<int>? categoryIds,
        List<int>? projectIds,
        string? sort,
        DateTime? dateFrom,
        DateTime? dateTo,
        CancellationToken cancellationToken);

    // ДОБАВЛЕННЫЕ МЕТОДЫ
    Task<bool> ExistsAsync(int projectId, CancellationToken cancellationToken);
    Task<string?> GetStatusAsync(int projectId, CancellationToken cancellationToken);
    Task<int?> GetCreatorUserIdAsync(int projectId, CancellationToken cancellationToken);
    Task<DateTime?> GetPublishedAtAsync(int projectId, CancellationToken cancellationToken);
    Task<decimal?> GetBudgetAsync(int projectId, CancellationToken cancellationToken);
    Task<int?> GetStatusIdByNameAsync(string name, CancellationToken cancellationToken);

    Task<List<int>> GetAllStatusIdsAsync(CancellationToken cancellationToken);
}