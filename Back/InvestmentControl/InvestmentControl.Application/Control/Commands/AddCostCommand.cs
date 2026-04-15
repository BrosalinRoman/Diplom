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
    private readonly IInvestmentRepository _investmentRepository;
    private readonly IProjectReadRepository _projectReadRepository;
    private readonly ICurrentUser _currentUser;

    public AddCostCommandHandler(
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

    public async Task<int> Handle(AddCostCommand request, CancellationToken cancellationToken)
    {
        if (request.ProjectId <= 0)
            throw new ArgumentException("ID проекта должен быть положительным.");

        if (_currentUser.Role != "Applicant" && _currentUser.Role != "Admin")
            throw new ForbiddenAccessException("Только заявитель или администратор может добавлять затраты.");

        if (!await _projectReadRepository.ExistsAsync(request.ProjectId, cancellationToken))
            throw new NotFoundException("Project", request.ProjectId);

        if (_currentUser.Role == "Applicant")
        {
            var creatorId = await _projectReadRepository.GetCreatorUserIdAsync(request.ProjectId, cancellationToken);
            if (creatorId != _currentUser.UserId)
                throw new ForbiddenAccessException("Вы можете добавлять затраты только для своих проектов.");
        }

        var status = await _projectReadRepository.GetStatusAsync(request.ProjectId, cancellationToken);
        if (status != "Активен" && status != "Завершен")
            throw new ArgumentException("Затраты можно добавлять только для активных или завершённых проектов.");

        var publishedAt = await _projectReadRepository.GetPublishedAtAsync(request.ProjectId, cancellationToken);
        if (request.Date < publishedAt)
            throw new ArgumentException("Дата затрат не может быть раньше даты публикации проекта.");

        var budget = await _projectReadRepository.GetBudgetAsync(request.ProjectId, cancellationToken) ?? 0;
        var existingCosts = (await _costRepository.GetByProjectIdAsync(request.ProjectId, cancellationToken)).ToList();
        var totalCosts = existingCosts.Sum(c => c.Amount) + request.Amount;
        if (totalCosts > budget)
            throw new ArgumentException($"Общая сумма затрат превышает бюджет на {totalCosts - budget}.");

        var investments = (await _investmentRepository.GetByProjectIdAsync(request.ProjectId, cancellationToken)).ToList();

        // Проверка на дату текущей затраты
        var investedUpToDate = investments
            .Where(i => i.ActualDate.HasValue && i.ActualDate <= request.Date)
            .Sum(i => i.ActualAmount ?? 0);
        var costsUpToDate = existingCosts.Where(c => c.Date <= request.Date).Sum(c => c.Amount) + request.Amount;
        if (costsUpToDate > investedUpToDate)
            throw new ArgumentException($"Затраты на дату {request.Date:d} превышают проинвестированную сумму на {costsUpToDate - investedUpToDate}. Сначала необходимо внести инвестиции.");

        // Проверка будущих затрат
        var futureCosts = existingCosts.Where(c => c.Date > request.Date).ToList();
        foreach (var futureCost in futureCosts)
        {
            var investedUpToFuture = investments
                .Where(i => i.ActualDate.HasValue && i.ActualDate <= futureCost.Date)
                .Sum(i => i.ActualAmount ?? 0);
            var costsUpToFuture = existingCosts.Where(c => c.Date <= futureCost.Date && c.Id != futureCost.Id).Sum(c => c.Amount) + futureCost.Amount;
            if (costsUpToFuture > investedUpToFuture)
            {
                var deficit = costsUpToFuture - investedUpToFuture;
                throw new ArgumentException($"Если добавить текущую затрату, то на дату {futureCost.Date:d} затрата ID={futureCost.Id} превысит инвестиции на {deficit}. Необходимо добавить инвестиции до этой даты или изменить дату затраты.");
            }
        }

        var cost = new Cost(request.ProjectId, request.Amount, request.Description, request.Responsible, request.Date);
        await _costRepository.AddAsync(cost, cancellationToken);
        await _costRepository.SaveChangesAsync(cancellationToken);
        return cost.Id;
    }
}