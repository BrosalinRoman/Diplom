// Application/Control/Commands/AddProgressReportCommand.cs
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
    private readonly ICurrentUser _currentUser;

    public AddProgressReportCommandHandler(IProgressReportRepository progressReportRepository, ICurrentUser currentUser)
    {
        _progressReportRepository = progressReportRepository;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(AddProgressReportCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.Role != "Applicant")
            throw new ForbiddenAccessException("Только заявитель может добавлять отчёты.");

        var report = new ProgressReport(request.ProjectId, request.Description, request.ProgressPercentage);
        await _progressReportRepository.AddAsync(report, cancellationToken);
        await _progressReportRepository.SaveChangesAsync(cancellationToken);

        return report.Id;
    }
}
