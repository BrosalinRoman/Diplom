using InvestmentControl.Application.Common.Helpers;
using InvestmentControl.Application.Common.Interfaces;
using InvestmentControl.Application.Control.DTOs;
using InvestmentControl.Domain.Interfaces;
using MediatR;

namespace InvestmentControl.Application.Control.Queries;

public class GetControlProjectsQuery : IRequest<List<ControlProjectDto>>
{
    public string? Search { get; set; }
    public List<int>? DirectionIds { get; set; }
    public List<int>? DepartmentIds { get; set; }
    public List<int>? CategoryIds { get; set; }
    public string? Sort { get; set; } 
}

public class GetControlProjectsQueryHandler : IRequestHandler<GetControlProjectsQuery, List<ControlProjectDto>>
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

    public async Task<List<ControlProjectDto>> Handle(GetControlProjectsQuery request, CancellationToken cancellationToken)
    {
        ValidationHelper.EnsurePositiveIds(request.DirectionIds, nameof(request.DirectionIds));
        ValidationHelper.EnsurePositiveIds(request.DepartmentIds, nameof(request.DepartmentIds));
        ValidationHelper.EnsurePositiveIds(request.CategoryIds, nameof(request.CategoryIds));

        // Валидация sort
        var allowedSortValues = new[] { "date_desc", "name_asc", "name_desc", "progress_desc", "progress_asc", "budget_desc", "budget_asc", "invested_desc", "invested_asc" };
        if (!string.IsNullOrEmpty(request.Sort) && !allowedSortValues.Contains(request.Sort))
            throw new ArgumentException($"Недопустимое значение sort. Допустимы: {string.Join(", ", allowedSortValues)}");

        var projectIds = _currentUser.Role == "Applicant"
            ? await _projectReadRepository.GetProjectIdsByCreatorAsync(_currentUser.UserId, cancellationToken)
            : null;

        var projects = await _projectReadRepository.GetControlProjectsAsync(
            request.Search,
            request.DirectionIds,
            request.DepartmentIds,
            request.CategoryIds,
            projectIds,
            request.Sort,
            cancellationToken);

        return projects.Select(p => new ControlProjectDto
        {
            Id = p.Id,
            Name = p.Name,
            Category = p.Category,
            Direction = p.Direction,
            Department = p.Department,
            Budget = p.Budget,
            Invested = p.Invested,
            Progress = p.Progress,
            StartDate = p.StartDate
        }).ToList();
    }
}
