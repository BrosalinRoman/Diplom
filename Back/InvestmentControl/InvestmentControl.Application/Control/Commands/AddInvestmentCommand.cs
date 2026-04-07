using InvestmentControl.Application.Common.Exceptions;
using InvestmentControl.Application.Common.Interfaces;
using InvestmentControl.Domain.Interfaces;
using InvestmentControl.Domain.Models;
using MediatR;

namespace InvestmentControl.Application.Control.Commands;

public class AddInvestmentCommand : IRequest<int>
{
    public int ProjectId { get; set; }
    public decimal? PlannedAmount { get; set; }
    public DateTime? PlannedDate { get; set; }
    public decimal? ActualAmount { get; set; }
    public DateTime? ActualDate { get; set; }
}

public class AddInvestmentCommandHandler : IRequestHandler<AddInvestmentCommand, int>
{
    private readonly IInvestmentRepository _investmentRepository;
    private readonly ICurrentUser _currentUser;

    public AddInvestmentCommandHandler(IInvestmentRepository investmentRepository, ICurrentUser currentUser)
    {
        _investmentRepository = investmentRepository;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(AddInvestmentCommand request, CancellationToken cancellationToken)
    {
        // Проверка прав: только инвестор может добавлять инвестиции
        if (_currentUser.Role != "Investor")
            throw new ForbiddenAccessException("Только инвестор может добавлять инвестиции.");

        // Дополнительно можно проверить, что проект существует и активен (через ProjectReadRepository)

        var investment = new Investment(
            request.ProjectId,
            request.PlannedAmount,
            request.PlannedDate,
            request.ActualAmount,
            request.ActualDate
        );

        await _investmentRepository.AddAsync(investment, cancellationToken);
        await _investmentRepository.SaveChangesAsync(cancellationToken);

        return investment.Id;
    }
}
