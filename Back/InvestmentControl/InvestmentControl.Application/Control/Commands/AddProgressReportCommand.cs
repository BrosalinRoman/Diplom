using InvestmentControl.Application.Common.Exceptions;
using InvestmentControl.Application.Common.Interfaces;
using InvestmentControl.Domain.Interfaces;
using InvestmentControl.Domain.Models;
using MediatR;

namespace InvestmentControl.Application.Control.Commands;

public class AddProgressReportCommand : IRequest<int>
{
    public int ProjectId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal ProgressPercentage { get; set; }
}

public class AddProgressReportCommandHandler : IRequestHandler<AddProgressReportCommand, int>
{
    private readonly IProgressReportRepository _progressReportRepository;
    private readonly IProjectReadRepository _projectReadRepository;
    private readonly ICurrentUser _currentUser;

    public AddProgressReportCommandHandler(IProgressReportRepository progressReportRepository, IProjectReadRepository projectReadRepository, ICurrentUser currentUser)
    {
        _progressReportRepository = progressReportRepository;
        _projectReadRepository = projectReadRepository;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(AddProgressReportCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.Role != "Applicant")
            throw new ForbiddenAccessException("Только заявитель может добавлять отчёты.");

        if (!await _projectReadRepository.ExistsAsync(request.ProjectId, cancellationToken))
            throw new NotFoundException("Project", request.ProjectId);

        var creatorId = await _projectReadRepository.GetCreatorUserIdAsync(request.ProjectId, cancellationToken);
        if (creatorId != _currentUser.UserId)
            throw new ForbiddenAccessException("Вы можете добавлять отчёты только для своих проектов.");

        var status = await _projectReadRepository.GetStatusAsync(request.ProjectId, cancellationToken);
        if (status != "Активен" && status != "Завершен")
            throw new ArgumentException("Отчёты можно добавлять только для активных или завершённых проектов.");

        var report = new ProgressReport(request.ProjectId, request.Description, request.ProgressPercentage);
        await _progressReportRepository.AddAsync(report, cancellationToken);
        await _progressReportRepository.SaveChangesAsync(cancellationToken);
        return report.Id;
    }
}