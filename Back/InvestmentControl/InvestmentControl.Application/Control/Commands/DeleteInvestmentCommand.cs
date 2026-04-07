using InvestmentControl.Application.Common.Exceptions;
using InvestmentControl.Application.Common.Interfaces;
using InvestmentControl.Domain.Interfaces;
using InvestmentControl.Domain.Models;
using MediatR;

public class DeleteInvestmentCommand : IRequest
{
    public int Id { get; set; }
}

public class DeleteInvestmentCommandHandler : IRequestHandler<DeleteInvestmentCommand>
{
    private readonly IInvestmentRepository _investmentRepository;
    private readonly ICurrentUser _currentUser;

    public DeleteInvestmentCommandHandler(IInvestmentRepository investmentRepository, ICurrentUser currentUser)
    {
        _investmentRepository = investmentRepository;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteInvestmentCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.Role != "Investor")
            throw new ForbiddenAccessException("Только инвестор может удалять инвестиции.");

        var investment = await _investmentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (investment == null)
            throw new NotFoundException(nameof(Investment), request.Id);

        _investmentRepository.Delete(investment);
        await _investmentRepository.SaveChangesAsync(cancellationToken);
    }
}
