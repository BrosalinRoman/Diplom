using InvestmentControl.Application.Common.DTOs;
using InvestmentControl.Application.Common.Helpers;
using InvestmentControl.Application.Common.Interfaces;
using InvestmentControl.Application.Control.DTOs;
using InvestmentControl.Domain.Interfaces;
using MediatR;

namespace InvestmentControl.Application.Control.Queries;

public class GetControlProjectsQuery : IRequest<PagedResponse<ControlProjectDto>>
{
    public string? Search { get; set; }
    public List<int>? DirectionIds { get; set; }
    public List<int>? DepartmentIds { get; set; }
    public List<int>? CategoryIds { get; set; }
    public List<int>? StatusIds { get; set; }
    public string? Sort { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetControlProjectsQueryHandler : IRequestHandler<GetControlProjectsQuery, PagedResponse<ControlProjectDto>>
{
    private readonly IProjectReadRepository _projectReadRepository;
    private readonly IInvestmentRepository _investmentRepository;
    private readonly IProgressReportRepository _progressReportRepository;
    private readonly ICurrentUser _currentUser;

    public GetControlProjectsQueryHandler(
        IProjectReadRepository projectReadRepository,
        IInvestmentRepository investmentRepository,
        IProgressReportRepository progressReportRepository,
        ICurrentUser currentUser)
    {
        _projectReadRepository = projectReadRepository;
        _investmentRepository = investmentRepository;
        _progressReportRepository = progressReportRepository;
        _currentUser = currentUser;
    }

    public async Task<PagedResponse<ControlProjectDto>> Handle(GetControlProjectsQuery request, CancellationToken cancellationToken)
    {
        // Проверка обязательности списков (хотя бы один элемент)
        ValidationHelper.EnsureNonEmptyList(request.DirectionIds, nameof(request.DirectionIds));
        ValidationHelper.EnsureNonEmptyList(request.DepartmentIds, nameof(request.DepartmentIds));
        ValidationHelper.EnsureNonEmptyList(request.CategoryIds, nameof(request.CategoryIds));

        ValidationHelper.EnsurePositiveIds(request.DirectionIds, nameof(request.DirectionIds));
        ValidationHelper.EnsurePositiveIds(request.DepartmentIds, nameof(request.DepartmentIds));
        ValidationHelper.EnsurePositiveIds(request.CategoryIds, nameof(request.CategoryIds));

        // Валидация sort
        var allowedSortValues = new[] { "date_desc", "name_asc", "name_desc", "progress_desc", "progress_asc", "budget_desc", "budget_asc", "invested_desc", "invested_asc" };
        if (!string.IsNullOrEmpty(request.Sort) && !allowedSortValues.Contains(request.Sort))
            throw new ArgumentException($"Недопустимое значение sort. Допустимы: {string.Join(", ", allowedSortValues)}");

        // Проверка, что StatusIds передан и не пуст
        if (request.StatusIds == null || !request.StatusIds.Any())
            throw new ArgumentException("Параметр StatusIds обязателен и должен содержать хотя бы один элемент.");

        // Получаем допустимые ID статусов "Активен" и "Завершен"
        var activeId = await _projectReadRepository.GetStatusIdByNameAsync("Активен", cancellationToken);
        var completedId = await _projectReadRepository.GetStatusIdByNameAsync("Завершен", cancellationToken);
        var allowedStatusIds = new List<int>();
        if (activeId.HasValue) allowedStatusIds.Add(activeId.Value);
        if (completedId.HasValue) allowedStatusIds.Add(completedId.Value);

        foreach (var statusId in request.StatusIds)
        {
            if (!allowedStatusIds.Contains(statusId))
                throw new ArgumentException($"Данный фильтр поддерживает только статусы 'Активен' (ID={activeId}) и 'Завершен' (ID={completedId}). Получен ID {statusId}.");
        }

        // Проверка дат
        if (request.DateFrom.HasValue && request.DateTo.HasValue && request.DateFrom > request.DateTo)
            throw new ArgumentException("DateFrom не может быть позже DateTo.");

        // Определяем, какие проекты видит пользователь
        var projectIds = _currentUser.Role == "Applicant"
            ? await _projectReadRepository.GetProjectIdsByCreatorAsync(_currentUser.UserId, cancellationToken)
            : null;

        var pagedResult = await _projectReadRepository.GetControlProjectsPagedAsync(
            request.Search,
            request.DirectionIds,
            request.DepartmentIds,
            request.CategoryIds,
            request.StatusIds,
            projectIds,
            request.Sort,
            request.DateFrom,
            request.DateTo,
            request.Page,
            request.PageSize,
            cancellationToken);

        var items = pagedResult.Items.Select(p => new ControlProjectDto
        {
            Id = p.Id,
            Name = p.Name,
            Category = p.Category,
            Direction = p.Direction,
            Department = p.Department,
            Budget = p.Budget,
            Invested = p.Invested,
            Progress = p.Progress,
            StartDate = p.StartDate,
            Status = p.Status
        }).ToList();

        return new PagedResponse<ControlProjectDto>
        {
            Items = items,
            Page = pagedResult.Page,
            PageSize = pagedResult.PageSize,
            TotalCount = pagedResult.TotalCount
        };
    }
}
