// Application/Control/Commands/UpdateInvestmentCommand.cs
using InvestmentControl.Application.Common.Exceptions;
using InvestmentControl.Application.Common.Interfaces;
using InvestmentControl.Domain.Interfaces;
using InvestmentControl.Domain.Models;
using MediatR;

namespace InvestmentControl.Application.Control.Commands;

public class UpdateInvestmentCommand : IRequest
{
    public int Id { get; set; }
    public decimal? PlannedAmount { get; set; }
    public DateTime? PlannedDate { get; set; }
    public decimal? ActualAmount { get; set; }
    public DateTime? ActualDate { get; set; }
}

public class UpdateInvestmentCommandHandler : IRequestHandler<UpdateInvestmentCommand>
{
    private readonly IInvestmentRepository _investmentRepository;
    private readonly ICurrentUser _currentUser;

    public UpdateInvestmentCommandHandler(IInvestmentRepository investmentRepository, ICurrentUser currentUser)
    {
        _investmentRepository = investmentRepository;
        _currentUser = currentUser;
    }

    public async Task Handle(UpdateInvestmentCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.Role != "Investor")
            throw new ForbiddenAccessException("Только инвестор может изменять инвестиции.");

        var investment = await _investmentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (investment == null)
            throw new NotFoundException(nameof(Investment), request.Id);

        investment.Update(request.PlannedAmount, request.PlannedDate, request.ActualAmount, request.ActualDate);
        _investmentRepository.Update(investment);
        await _investmentRepository.SaveChangesAsync(cancellationToken);
    }
}
