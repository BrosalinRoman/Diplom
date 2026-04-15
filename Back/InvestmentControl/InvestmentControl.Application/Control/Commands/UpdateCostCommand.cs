using InvestmentControl.Application.Common.Exceptions;
using InvestmentControl.Application.Common.Interfaces;
using InvestmentControl.Application.Control.DTOs;
using InvestmentControl.Domain.Interfaces;
using InvestmentControl.Domain.Models;
using MediatR;

namespace InvestmentControl.Application.Control.Commands;

public class UpdateCostCommand : IRequest<CostDto>
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Responsible { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}

public class UpdateCostCommandHandler : IRequestHandler<UpdateCostCommand, CostDto>
{
    private readonly ICostRepository _costRepository;
    private readonly IInvestmentRepository _investmentRepository;
    private readonly IProjectReadRepository _projectReadRepository;
    private readonly ICurrentUser _currentUser;

    public UpdateCostCommandHandler(
        ICostRepository costRepository,
        IInvestmentRepository investmentRepository,
        IProjectReadRepository projectReadRepository,
        ICurrentUser currentUser)
    {
        _costRepository = costRepository;
        _investmentRepository = investmentRepository;
        _projectReadRepository = projectReadRepository;
        _currentUser = currentUser;
    }

    public async Task<CostDto> Handle(UpdateCostCommand request, CancellationToken cancellationToken)
    {
        if (request.Id <= 0)
            throw new ArgumentException("ID затраты должен быть положительным.");

        if (_currentUser.Role != "Applicant" && _currentUser.Role != "Admin")
            throw new ForbiddenAccessException("Только заявитель или администратор может изменять затраты.");

        var cost = await _costRepository.GetByIdAsync(request.Id, cancellationToken);
        if (cost == null)
            throw new NotFoundException("Cost", request.Id);

        if (_currentUser.Role == "Applicant")
        {
            var creatorId = await _projectReadRepository.GetCreatorUserIdAsync(cost.ProjectId, cancellationToken);
            if (creatorId != _currentUser.UserId)
                throw new ForbiddenAccessException("Вы можете изменять затраты только для своих проектов.");
        }

        var status = await _projectReadRepository.GetStatusAsync(cost.ProjectId, cancellationToken);
        if (status != "Активен" && status != "Завершен")
            throw new ArgumentException("Затраты можно изменять только для активных или завершённых проектов.");

        var publishedAt = await _projectReadRepository.GetPublishedAtAsync(cost.ProjectId, cancellationToken);
        if (request.Date < publishedAt)
            throw new ArgumentException("Дата затрат не может быть раньше даты публикации проекта.");

        var budget = await _projectReadRepository.GetBudgetAsync(cost.ProjectId, cancellationToken) ?? 0;
        var allCosts = (await _costRepository.GetByProjectIdAsync(cost.ProjectId, cancellationToken)).ToList();
        var otherCosts = allCosts.Where(c => c.Id != request.Id).ToList();
        var totalCosts = otherCosts.Sum(c => c.Amount) + request.Amount;
        if (totalCosts > budget)
            throw new ArgumentException($"Общая сумма затрат превышает бюджет на {totalCosts - budget}.");

        var investments = (await _investmentRepository.GetByProjectIdAsync(cost.ProjectId, cancellationToken)).ToList();

        // Проверка на дату редактируемой затраты
        var investedUpToDate = investments
            .Where(i => i.ActualDate.HasValue && i.ActualDate <= request.Date)
            .Sum(i => i.ActualAmount ?? 0);
        var costsUpToDate = otherCosts.Where(c => c.Date <= request.Date).Sum(c => c.Amount) + request.Amount;
        if (costsUpToDate > investedUpToDate)
            throw new ArgumentException($"Затраты на дату {request.Date:d} превышают проинвестированную сумму на {costsUpToDate - investedUpToDate}. Сначала необходимо внести инвестиции.");

        // Проверка будущих затрат
        var futureCosts = allCosts.Where(c => c.Date > request.Date).ToList();
        foreach (var futureCost in futureCosts)
        {
            var investedUpToFuture = investments
                .Where(i => i.ActualDate.HasValue && i.ActualDate <= futureCost.Date)
                .Sum(i => i.ActualAmount ?? 0);
            var costsUpToFuture = otherCosts.Where(c => c.Date <= futureCost.Date && c.Id != futureCost.Id).Sum(c => c.Amount)
                                  + (futureCost.Id == request.Id ? request.Amount : futureCost.Amount);
            if (costsUpToFuture > investedUpToFuture)
            {
                var deficit = costsUpToFuture - investedUpToFuture;
                throw new ArgumentException($"Если изменить текущую затрату, то на дату {futureCost.Date:d} затрата ID={futureCost.Id} превысит инвестиции на {deficit}. Необходимо добавить инвестиции до этой даты или изменить дату затраты.");
            }
        }

        cost.Update(request.Amount, request.Description, request.Responsible, request.Date);
        _costRepository.Update(cost);
        await _costRepository.SaveChangesAsync(cancellationToken);

        return new CostDto
        {
            Id = cost.Id,
            Amount = cost.Amount,
            Description = cost.Description,
            Responsible = cost.Responsible,
            Date = cost.Date
        };
    }
}
