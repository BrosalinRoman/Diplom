using InvestmentControl.Application.Common.Exceptions;
using InvestmentControl.Application.Common.Interfaces;
using InvestmentControl.Application.Control.DTOs;
using InvestmentControl.Domain.Interfaces;
using MediatR;

namespace InvestmentControl.Application.Control.Queries;

public class GetProgressReportsQuery : IRequest<List<ProgressReportDto>>
{
    public int ProjectId { get; set; }
}

public class GetProgressReportsQueryHandler : IRequestHandler<GetProgressReportsQuery, List<ProgressReportDto>>
{
    private readonly IProgressReportRepository _progressReportRepository;
    private readonly IProjectReadRepository _projectReadRepository;
    private readonly ICurrentUser _currentUser;

    public GetProgressReportsQueryHandler(
        IProgressReportRepository progressReportRepository,
        IProjectReadRepository projectReadRepository,
        ICurrentUser currentUser)
    {
        _progressReportRepository = progressReportRepository;
        _projectReadRepository = projectReadRepository;
        _currentUser = currentUser;
    }

    public async Task<List<ProgressReportDto>> Handle(GetProgressReportsQuery request, CancellationToken cancellationToken)
    {
        var status = await _projectReadRepository.GetStatusAsync(request.ProjectId, cancellationToken);
        if (status != "Активен" && status != "Завершен")
            throw new ArgumentException("На данный момент проект не Активен и не является Завершенным.");

        if (request.ProjectId <= 0)
            throw new ArgumentException("ID проекта должен быть положительным.");

        if (!await _projectReadRepository.ExistsAsync(request.ProjectId, cancellationToken))
            throw new NotFoundException("Project", request.ProjectId);

        if (_currentUser.Role == "Applicant")
        {
            var creatorId = await _projectReadRepository.GetCreatorUserIdAsync(request.ProjectId, cancellationToken);
            if (creatorId != _currentUser.UserId)
                throw new ForbiddenAccessException("Вы не можете просматривать отчёты чужих проектов.");
        }

        var reports = await _progressReportRepository.GetByProjectIdAsync(request.ProjectId, cancellationToken);
        return reports.Select(r => new ProgressReportDto
        {
            Id = r.Id,
            Description = r.Description,
            ProgressPercentage = r.ProgressPercentage,
            ReportDate = r.ReportDate
        }).ToList();
    }
}
