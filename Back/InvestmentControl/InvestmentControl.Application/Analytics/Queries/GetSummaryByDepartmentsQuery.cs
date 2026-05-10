using InvestmentControl.Application.Analytics.DTOs;
using InvestmentControl.Application.Common.Exceptions;
using InvestmentControl.Application.Common.Helpers;
using InvestmentControl.Application.Common.Interfaces;
using InvestmentControl.Domain.Interfaces;
using MediatR;

namespace InvestmentControl.Application.Analytics.Queries;

public class GetSummaryByDepartmentsQuery : IRequest<List<DepartmentSummaryDto>>
{
    public List<int>? DepartmentIds { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public List<int>? StatusIds { get; set; }
    public List<int>? DirectionIds { get; set; }
    public List<int>? CategoryIds { get; set; }
}

public class GetSummaryByDepartmentsQueryHandler : IRequestHandler<GetSummaryByDepartmentsQuery, List<DepartmentSummaryDto>>
{
    private readonly IProjectReadRepository _projectReadRepository;
    private readonly ICurrentUser _currentUser;

    public GetSummaryByDepartmentsQueryHandler(IProjectReadRepository projectReadRepository, ICurrentUser currentUser)
    {
        _projectReadRepository = projectReadRepository;
        _currentUser = currentUser;
    }

    public async Task<List<DepartmentSummaryDto>> Handle(GetSummaryByDepartmentsQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.Role != "Investor" && _currentUser.Role != "Admin")
            throw new ForbiddenAccessException("Только инвестор или администратор может просматривать сводку.");

        ValidationHelper.EnsureNonEmptyList(request.DepartmentIds, nameof(request.DepartmentIds));
        ValidationHelper.EnsureNonEmptyList(request.StatusIds, nameof(request.StatusIds));
        ValidationHelper.EnsureNonEmptyList(request.DirectionIds, nameof(request.DirectionIds));
        ValidationHelper.EnsureNonEmptyList(request.CategoryIds, nameof(request.CategoryIds));

        ValidationHelper.EnsurePositiveIds(request.DepartmentIds, nameof(request.DepartmentIds));
        ValidationHelper.EnsurePositiveIds(request.StatusIds, nameof(request.StatusIds));
        ValidationHelper.EnsurePositiveIds(request.DirectionIds, nameof(request.DirectionIds));
        ValidationHelper.EnsurePositiveIds(request.CategoryIds, nameof(request.CategoryIds));

        if (request.DateFrom.HasValue && request.DateTo.HasValue && request.DateFrom > request.DateTo)
            throw new ArgumentException("DateFrom не может быть позже DateTo.");

        // Получаем ID статуса "Черновик"
        var draftStatusId = await _projectReadRepository.GetStatusIdByNameAsync("Черновик", cancellationToken);

        // Если черновик явно выбран в фильтре – ошибка
        if (request.StatusIds != null && draftStatusId.HasValue && request.StatusIds.Contains(draftStatusId.Value))
            throw new ArgumentException("Статус 'Черновик' не может быть использован в сводке.");

        // Формируем список разрешённых статусов
        List<int> allowedStatusIds;
        if (request.StatusIds == null || !request.StatusIds.Any())
        {
            // Если статусы не указаны – берём все статусы, кроме черновика
            var allStatuses = await _projectReadRepository.GetAllStatusIdsAsync(cancellationToken);
            allowedStatusIds = allStatuses.Where(id => id != draftStatusId).ToList();
        }
        else
        {
            allowedStatusIds = request.StatusIds.Where(id => id != draftStatusId).ToList();
        }

        // Если после исключения черновика список пуст – возвращаем пустой результат
        if (!allowedStatusIds.Any())
            return new List<DepartmentSummaryDto>();

        var summary = await _projectReadRepository.GetDepartmentSummaryAsync(
            request.DepartmentIds,
            request.DateFrom,
            request.DateTo,
            allowedStatusIds,
            request.DirectionIds,
            request.CategoryIds,
            cancellationToken);

        return summary.Select(s => new DepartmentSummaryDto
        {
            DepartmentId = s.DepartmentId,
            DepartmentName = s.DepartmentName,
            ProjectCount = s.ProjectCount,
            TotalBudget = s.TotalBudget
        }).ToList();
    }
}