using InvestmentControl.Application.Common.Exceptions;
using InvestmentControl.Application.Common.Interfaces;
using InvestmentControl.Domain.Interfaces;
using InvestmentControl.Domain.Models;
using MediatR;

public class DeleteCostCommand : IRequest
{
    public int Id { get; set; }
}

public class DeleteCostCommandHandler : IRequestHandler<DeleteCostCommand>
{
    private readonly ICostRepository _costRepository;
    private readonly ICurrentUser _currentUser;

    public DeleteCostCommandHandler(ICostRepository costRepository, ICurrentUser currentUser)
    {
        _costRepository = costRepository;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteCostCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.Role != "Applicant")
            throw new ForbiddenAccessException("Только заявитель может удалять затраты.");

        var cost = await _costRepository.GetByIdAsync(request.Id, cancellationToken);
        if (cost == null)
            throw new NotFoundException(nameof(Cost), request.Id);

        _costRepository.Delete(cost);
        await _costRepository.SaveChangesAsync(cancellationToken);
    }
}
