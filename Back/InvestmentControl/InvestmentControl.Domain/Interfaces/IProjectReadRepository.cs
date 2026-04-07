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
        List<int>? projectIds,
        string? search,
        CancellationToken cancellationToken = default);

    Task<List<int>> GetProjectIdsByCreatorAsync(int userId, CancellationToken cancellationToken = default);

    Task<List<DepartmentSummaryReadModel>> GetDepartmentSummaryAsync(
        List<int>? departmentIds,
        DateTime? dateFrom,
        DateTime? dateTo,
        List<int>? statusIds,
        List<int>? directionIds,
        List<int>? categoryIds,
        int? budgetFieldId,
        CancellationToken cancellationToken = default);

    Task<List<ControlProjectReadModel>> GetControlProjectsAsync(
        string? search,
        List<int>? directionIds,
        List<int>? departmentIds,
        List<int>? categoryIds,
        List<int>? projectIds,
        string? sort,
        CancellationToken cancellationToken = default);
}