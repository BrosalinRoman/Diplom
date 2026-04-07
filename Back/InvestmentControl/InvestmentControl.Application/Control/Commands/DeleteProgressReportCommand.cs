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
    private readonly IProgressReportRepository _repository;
    private readonly ICurrentUser _currentUser;

    public DeleteProgressReportCommandHandler(IProgressReportRepository repository, ICurrentUser currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteProgressReportCommand request, CancellationToken cancellationToken)
    {
        // Только заявитель может удалять отчёты
        if (_currentUser.Role != "Applicant")
            throw new ForbiddenAccessException("Только заявитель может удалять отчёты.");

        var report = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (report == null)
            throw new NotFoundException(nameof(ProgressReport), request.Id);

        _repository.Delete(report);
        await _repository.SaveChangesAsync(cancellationToken);
    }
}
