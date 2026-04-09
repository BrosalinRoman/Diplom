using InvestmentControl.Application.Common.Exceptions;
using InvestmentControl.Application.Common.Interfaces;
using InvestmentControl.Domain.Interfaces;
using InvestmentControl.Domain.Models;
using MediatR;

namespace InvestmentControl.Application.Control.Commands;

public class DeleteInvestmentCommand : IRequest
{
    public int Id { get; set; }
}

public class DeleteInvestmentCommandHandler : IRequestHandler<DeleteInvestmentCommand>
{
    private readonly IInvestmentRepository _investmentRepository;
    private readonly IProjectReadRepository _projectReadRepository;
    private readonly ICurrentUser _currentUser;

    public DeleteInvestmentCommandHandler(IInvestmentRepository investmentRepository, IProjectReadRepository projectReadRepository, ICurrentUser currentUser)
    {
        _investmentRepository = investmentRepository;
        _projectReadRepository = projectReadRepository;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteInvestmentCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.Role != "Investor")
            throw new ForbiddenAccessException("Только инвестор может удалять инвестиции.");

        var investment = await _investmentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (investment == null)
            throw new NotFoundException(nameof(Investment), request.Id);

        // Проверяем существование проекта (опционально)
        if (!await _projectReadRepository.ExistsAsync(investment.ProjectId, cancellationToken))
            throw new NotFoundException("Project", investment.ProjectId);

        _investmentRepository.Delete(investment);
        await _investmentRepository.SaveChangesAsync(cancellationToken);
    }
}