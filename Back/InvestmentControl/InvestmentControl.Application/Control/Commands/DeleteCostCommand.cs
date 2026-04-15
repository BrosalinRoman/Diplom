using InvestmentControl.Application.Common.Exceptions;
using InvestmentControl.Application.Common.Interfaces;
using InvestmentControl.Domain.Interfaces;
using InvestmentControl.Domain.Models;
using MediatR;

namespace InvestmentControl.Application.Control.Commands;

public class DeleteCostCommand : IRequest
{
    public int Id { get; set; }
}

public class DeleteCostCommandHandler : IRequestHandler<DeleteCostCommand>
{
    private readonly ICostRepository _costRepository;
    private readonly IProjectReadRepository _projectReadRepository;
    private readonly ICurrentUser _currentUser;

    public DeleteCostCommandHandler(
        ICostRepository costRepository,
        IProjectReadRepository projectReadRepository,
        ICurrentUser currentUser)
    {
        _costRepository = costRepository;
        _projectReadRepository = projectReadRepository;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteCostCommand request, CancellationToken cancellationToken)
    {
        if (request.Id <= 0)
            throw new ArgumentException("ID затраты должен быть положительным.");

        if (_currentUser.Role != "Applicant" && _currentUser.Role != "Admin")
            throw new ForbiddenAccessException("Только заявитель или администратор может удалять затраты.");

        var cost = await _costRepository.GetByIdAsync(request.Id, cancellationToken);
        if (cost == null)
            throw new NotFoundException("Cost", request.Id);

        if (_currentUser.Role == "Applicant")
        {
            var creatorId = await _projectReadRepository.GetCreatorUserIdAsync(cost.ProjectId, cancellationToken);
            if (creatorId != _currentUser.UserId)
                throw new ForbiddenAccessException("Вы можете удалять затраты только для своих проектов.");
        }

        var status = await _projectReadRepository.GetStatusAsync(cost.ProjectId, cancellationToken);
        if (status != "Активен")
            throw new ArgumentException("Затраты можно удалять только для активных проектов.");

        _costRepository.Delete(cost);
        await _costRepository.SaveChangesAsync(cancellationToken);
    }
}