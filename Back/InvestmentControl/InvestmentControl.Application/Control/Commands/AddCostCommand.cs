using InvestmentControl.Application.Common.Exceptions;
using InvestmentControl.Application.Common.Interfaces;
using InvestmentControl.Domain.Interfaces;
using InvestmentControl.Domain.Models;
using MediatR;

namespace InvestmentControl.Application.Control.Commands;

public class AddCostCommand : IRequest<int>
{
    public int ProjectId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Responsible { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}

public class AddCostCommandHandler : IRequestHandler<AddCostCommand, int>
{
    private readonly ICostRepository _costRepository;
    private readonly ICurrentUser _currentUser;

    public AddCostCommandHandler(ICostRepository costRepository, ICurrentUser currentUser)
    {
        _costRepository = costRepository;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(AddCostCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.Role != "Applicant")
            throw new ForbiddenAccessException("Только заявитель может добавлять затраты.");

        var cost = new Cost(request.ProjectId, request.Amount, request.Description, request.Responsible, request.Date);
        await _costRepository.AddAsync(cost, cancellationToken);
        await _costRepository.SaveChangesAsync(cancellationToken);

        return cost.Id;
    }
}
