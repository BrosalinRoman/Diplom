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
    public string? Sort { get; set; } // "date_desc", "name_asc", "progress_desc", "progress_asc"
}

public class GetControlProjectsQueryHandler : IRequestHandler<GetControlProjectsQuery, List<ControlProjectDto>>
{
    private readonly IProjectReadRepository _projectReadRepository;
    private readonly IInvestmentRepository _investmentRepository;
    private readonly IProgressReportRepository _progressReportRepository;
    private readonly ICurrentUser _currentUser;

    public GetControlProjectsQueryHandler(IProjectReadRepository projectReadRepository, IInvestmentRepository investmentRepository, IProgressReportRepository progressReportRepository, ICurrentUser currentUser)
    {
        _projectReadRepository = projectReadRepository;
        _investmentRepository = investmentRepository;
        _progressReportRepository = progressReportRepository;
        _currentUser = currentUser;
    }

    public async Task<List<ControlProjectDto>> Handle(GetControlProjectsQuery request, CancellationToken cancellationToken)
    {
        // Определяем видимость проектов
        var projectIds = _currentUser.Role == "Applicant"
            ? await _projectReadRepository.GetProjectIdsByCreatorAsync(_currentUser.UserId, cancellationToken)
            : null; // null = все проекты

        var projects = await _projectReadRepository.GetControlProjectsAsync(
            request.Search,
            request.DirectionIds,
            request.DepartmentIds,
            request.CategoryIds,
            projectIds,
            request.Sort,
            cancellationToken);

        var result = new List<ControlProjectDto>();

        foreach (var project in projects)
        {
            // Получаем инвестиции для этого проекта
            var investments = await _investmentRepository.GetByProjectIdAsync(project.Id, cancellationToken);
            decimal invested = investments.Where(i => i.ActualAmount.HasValue).Sum(i => i.ActualAmount.Value);

            // Получаем последний отчёт по прогрессу
            var reports = await _progressReportRepository.GetByProjectIdAsync(project.Id, cancellationToken);
            decimal progress = reports.Any() ? reports.Max(r => r.ProgressPercentage) : 0;

            result.Add(new ControlProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Category = project.Category,
                Direction = project.Direction,
                Department = project.Department,
                Budget = project.Budget,
                Invested = invested,
                Progress = progress,
                StartDate = project.StartDate
            });
        }

        return result;
    }
}
