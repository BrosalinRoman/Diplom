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

    public DeleteInvestmentCommandHandler(
        IInvestmentRepository investmentRepository,
        IProjectReadRepository projectReadRepository,
        ICurrentUser currentUser)
    {
        _investmentRepository = investmentRepository;
        _projectReadRepository = projectReadRepository;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteInvestmentCommand request, CancellationToken cancellationToken)
    {
        if (request.Id <= 0)
            throw new ArgumentException("ID инвестиции должен быть положительным.");

        if (_currentUser.Role != "Investor" && _currentUser.Role != "Admin")
            throw new ForbiddenAccessException("Только инвестор или администратор может удалять инвестиции.");

        var investment = await _investmentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (investment == null)
            throw new NotFoundException("Investment", request.Id);

        var status = await _projectReadRepository.GetStatusAsync(investment.ProjectId, cancellationToken);
        if (status != "Активен")
            throw new ArgumentException("Инвестиции можно удалять только для активных проектов.");

        _investmentRepository.Delete(investment);
        await _investmentRepository.SaveChangesAsync(cancellationToken);
    }
}