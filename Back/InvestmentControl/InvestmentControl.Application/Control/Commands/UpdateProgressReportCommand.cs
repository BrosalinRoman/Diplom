using InvestmentControl.Application.Common.Exceptions;
using InvestmentControl.Application.Common.Interfaces;
using InvestmentControl.Domain.Interfaces;
using InvestmentControl.Domain.Models;
using MediatR;

namespace InvestmentControl.Application.Control.Commands;

public class UpdateProgressReportCommand : IRequest
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal ProgressPercentage { get; set; }
}

public class UpdateProgressReportCommandHandler : IRequestHandler<UpdateProgressReportCommand>
{
    private readonly IProgressReportRepository _progressReportRepository;
    private readonly IProjectReadRepository _projectReadRepository;
    private readonly ICurrentUser _currentUser;

    public UpdateProgressReportCommandHandler(IProgressReportRepository progressReportRepository, IProjectReadRepository projectReadRepository, ICurrentUser currentUser)
    {
        _progressReportRepository = progressReportRepository;
        _projectReadRepository = projectReadRepository;
        _currentUser = currentUser;
    }

    public async Task Handle(UpdateProgressReportCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.Role != "Applicant")
            throw new ForbiddenAccessException("Только заявитель может изменять отчёты.");

        var report = await _progressReportRepository.GetByIdAsync(request.Id, cancellationToken);
        if (report == null)
            throw new NotFoundException(nameof(ProgressReport), request.Id);

        var creatorId = await _projectReadRepository.GetCreatorUserIdAsync(report.ProjectId, cancellationToken);
        if (creatorId != _currentUser.UserId)
            throw new ForbiddenAccessException("Вы можете изменять отчёты только для своих проектов.");

        report.Update(request.Description, request.ProgressPercentage);
        _progressReportRepository.Update(report);
        await _progressReportRepository.SaveChangesAsync(cancellationToken);
    }
}