using InvestmentControl.Application.Common.Exceptions;
using InvestmentControl.Application.Common.Interfaces;
using InvestmentControl.Application.Control.DTOs;
using InvestmentControl.Domain.Interfaces;
using MediatR;

namespace InvestmentControl.Application.Control.Queries;

public class GetCostsQuery : IRequest<List<CostDto>>
{
    public int ProjectId { get; set; }
}

public class GetCostsQueryHandler : IRequestHandler<GetCostsQuery, List<CostDto>>
{
    private readonly ICostRepository _costRepository;
    private readonly IProjectReadRepository _projectReadRepository;
    private readonly ICurrentUser _currentUser;

    public GetCostsQueryHandler(
        ICostRepository costRepository,
        IProjectReadRepository projectReadRepository,
        ICurrentUser currentUser)
    {
        _costRepository = costRepository;
        _projectReadRepository = projectReadRepository;
        _currentUser = currentUser;
    }

    public async Task<List<CostDto>> Handle(GetCostsQuery request, CancellationToken cancellationToken)
    {
        var status = await _projectReadRepository.GetStatusAsync(request.ProjectId, cancellationToken);
        if (status != "Активен" && status != "Завершен")
            throw new ArgumentException("На данный момент проект не Активен и не является Завершенным.");

        if (request.ProjectId <= 0)
            throw new ArgumentException("ID проекта должен быть положительным.");

        if (!await _projectReadRepository.ExistsAsync(request.ProjectId, cancellationToken))
            throw new NotFoundException("Project", request.ProjectId);

        if (_currentUser.Role == "Applicant")
        {
            var creatorId = await _projectReadRepository.GetCreatorUserIdAsync(request.ProjectId, cancellationToken);
            if (creatorId != _currentUser.UserId)
                throw new ForbiddenAccessException("Вы не можете просматривать затраты чужих проектов.");
        }

        var costs = await _costRepository.GetByProjectIdAsync(request.ProjectId, cancellationToken);
        return costs.Select(c => new CostDto
        {
            Id = c.Id,
            Amount = c.Amount,
            Description = c.Description,
            Responsible = c.Responsible,
            Date = c.Date
        }).ToList();
    }
}