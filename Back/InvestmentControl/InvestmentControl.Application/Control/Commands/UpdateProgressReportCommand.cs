using InvestmentControl.Application.Common.Exceptions;
using InvestmentControl.Application.Common.Interfaces;
using InvestmentControl.Domain.Interfaces;
using InvestmentControl.Domain.Models;
using MediatR;

public class UpdateProgressReportCommand : IRequest
{
    public int Id { get; set; }
    public string? Description { get; set; }
    public decimal ProgressPercentage { get; set; }
}

public class UpdateProgressReportCommandHandler : IRequestHandler<UpdateProgressReportCommand>
{
    private readonly IProgressReportRepository _repository;
    private readonly ICurrentUser _currentUser;

    public UpdateProgressReportCommandHandler(IProgressReportRepository repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task Handle(UpdateProgressReportCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.Role != "Applicant")
            throw new ForbiddenAccessException("Только заявитель может изменять отчёты.");

        var report = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (report == null)
            throw new NotFoundException(nameof(ProgressReport), request.Id);

        report.Update(request.Description, request.ProgressPercentage);
        _repository.Update(report);
        await _repository.SaveChangesAsync(cancellationToken);
    }
}