using InvestmentControl.Application.Common.Exceptions;
using InvestmentControl.Application.Common.Interfaces;
using InvestmentControl.Domain.Interfaces;
using InvestmentControl.Domain.Models;
using MediatR;

public class UpdateCostCommand : IRequest
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Responsible { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}

public class UpdateCostCommandHandler : IRequestHandler<UpdateCostCommand>
{
    private readonly ICostRepository _costRepository;
    private readonly ICurrentUser _currentUser;

    public UpdateCostCommandHandler(ICostRepository costRepository, ICurrentUser currentUser)
    {
        _costRepository = costRepository;
        _currentUser = currentUser;
    }

    public async Task Handle(UpdateCostCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.Role != "Applicant")
            throw new ForbiddenAccessException("Только заявитель может изменять затраты.");

        var cost = await _costRepository.GetByIdAsync(request.Id, cancellationToken);
        if (cost == null)
            throw new NotFoundException(nameof(Cost), request.Id);

        cost.Update(request.Amount, request.Description, request.Responsible, request.Date);
        _costRepository.Update(cost);
        await _costRepository.SaveChangesAsync(cancellationToken);
    }
}
