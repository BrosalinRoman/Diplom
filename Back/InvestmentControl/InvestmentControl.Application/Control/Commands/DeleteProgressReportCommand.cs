using InvestmentControl.Application.Common.Exceptions;
using InvestmentControl.Application.Common.Interfaces;
using InvestmentControl.Domain.Interfaces;
using InvestmentControl.Domain.Models;
using MediatR;

namespace InvestmentControl.Application.Control.Commands;

public class DeleteProgressReportCommand : IRequest
{
    public int Id { get; set; }
}

public class DeleteProgressReportCommandHandler : IRequestHandler<DeleteProgressReportCommand>
{
    private readonly IProgressReportRepository _progressReportRepository;
    private readonly IProjectReadRepository _projectReadRepository;
    private readonly ICurrentUser _currentUser;

    public DeleteProgressReportCommandHandler(
        IProgressReportRepository progressReportRepository,
        IProjectReadRepository projectReadRepository,
        ICurrentUser currentUser)
    {
        _progressReportRepository = progressReportRepository;
        _projectReadRepository = projectReadRepository;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteProgressReportCommand request, CancellationToken cancellationToken)
    {
        if (request.Id <= 0)
            throw new ArgumentException("ID отчёта должен быть положительным.");

        if (_currentUser.Role != "Applicant" && _currentUser.Role != "Admin")
            throw new ForbiddenAccessException("Только заявитель или администратор может удалять отчёты.");

        var report = await _progressReportRepository.GetByIdAsync(request.Id, cancellationToken);
        if (report == null)
            throw new NotFoundException("ProgressReport", request.Id);

        if (_currentUser.Role == "Applicant")
        {
            var creatorId = await _projectReadRepository.GetCreatorUserIdAsync(report.ProjectId, cancellationToken);
            if (creatorId != _currentUser.UserId)
                throw new ForbiddenAccessException("Вы можете удалять отчёты только для своих проектов.");
        }

        var status = await _projectReadRepository.GetStatusAsync(report.ProjectId, cancellationToken);
        if (status != "Активен")
            throw new ArgumentException("Отчёты можно удалять только для активных проектов.");

        _progressReportRepository.Delete(report);
        await _progressReportRepository.SaveChangesAsync(cancellationToken);
    }
}