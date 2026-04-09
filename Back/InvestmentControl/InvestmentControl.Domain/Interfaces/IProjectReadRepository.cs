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
        string? search,
        CancellationToken cancellationToken);

    Task<List<int>> GetProjectIdsByCreatorAsync(int userId, CancellationToken cancellationToken);

    Task<List<DepartmentSummaryReadModel>> GetDepartmentSummaryAsync(
        List<int>? departmentIds,
        DateTime? dateFrom,
        DateTime? dateTo,
        List<int>? statusIds,
        List<int>? directionIds,
        List<int>? categoryIds,
        int? budgetFieldId,
        CancellationToken cancellationToken);

    Task<List<ControlProjectReadModel>> GetControlProjectsAsync(
        string? search,
        List<int>? directionIds,
        List<int>? departmentIds,
        List<int>? categoryIds,
        List<int>? projectIds,
        string? sort,
        CancellationToken cancellationToken);

    // ДОБАВЛЕННЫЕ МЕТОДЫ
    Task<bool> ExistsAsync(int projectId, CancellationToken cancellationToken);
    Task<string?> GetStatusAsync(int projectId, CancellationToken cancellationToken);
    Task<int?> GetCreatorUserIdAsync(int projectId, CancellationToken cancellationToken);
}