using InvestmentControl.Application.Analytics.DTOs;
using InvestmentControl.Application.Common.Exceptions;
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
    public int? BudgetFieldId { get; set; } // ID характеристики, используемой как бюджет
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
        // ИЗМЕНЕНО: доступ для Investor или Admin
        if (_currentUser.Role != "Investor" && _currentUser.Role != "Admin")
            throw new ForbiddenAccessException("Только инвестор или администратор может просматривать сводку по подразделениям.");

        var summaryReadModels = await _projectReadRepository.GetDepartmentSummaryAsync(
            request.DepartmentIds,
            request.DateFrom,
            request.DateTo,
            request.StatusIds,
            request.DirectionIds,
            request.CategoryIds,
            request.BudgetFieldId,
            cancellationToken);

        var result = summaryReadModels.Select(s => new DepartmentSummaryDto
        {
            DepartmentId = s.DepartmentId,
            DepartmentName = s.DepartmentName,
            ProjectCount = s.ProjectCount,
            TotalBudget = s.TotalBudget
        }).ToList();

        return result;
    }
}