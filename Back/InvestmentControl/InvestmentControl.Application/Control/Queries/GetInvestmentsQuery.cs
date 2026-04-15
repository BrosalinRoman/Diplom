using InvestmentControl.Application.Common.Exceptions;
using InvestmentControl.Application.Common.Interfaces;
using InvestmentControl.Application.Control.DTOs;
using InvestmentControl.Domain.Interfaces;
using MediatR;

namespace InvestmentControl.Application.Control.Queries;

public class GetInvestmentsQuery : IRequest<List<InvestmentDto>>
{
    public int ProjectId { get; set; }
}

public class GetInvestmentsQueryHandler : IRequestHandler<GetInvestmentsQuery, List<InvestmentDto>>
{
    private readonly IInvestmentRepository _investmentRepository;
    private readonly IProjectReadRepository _projectReadRepository;
    private readonly ICurrentUser _currentUser;

    public GetInvestmentsQueryHandler(
        IInvestmentRepository investmentRepository,
        IProjectReadRepository projectReadRepository,
        ICurrentUser currentUser)
    {
        _investmentRepository = investmentRepository;
        _projectReadRepository = projectReadRepository;
        _currentUser = currentUser;
    }

    public async Task<List<InvestmentDto>> Handle(GetInvestmentsQuery request, CancellationToken cancellationToken)
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
                throw new ForbiddenAccessException("Вы не можете просматривать инвестиции чужих проектов.");
        }

        var investments = await _investmentRepository.GetByProjectIdAsync(request.ProjectId, cancellationToken);
        return investments.Select(i => new InvestmentDto
        {
            Id = i.Id,
            PlannedAmount = i.PlannedAmount,
            PlannedDate = i.PlannedDate,
            ActualAmount = i.ActualAmount,
            ActualDate = i.ActualDate
        }).ToList();
    }
}