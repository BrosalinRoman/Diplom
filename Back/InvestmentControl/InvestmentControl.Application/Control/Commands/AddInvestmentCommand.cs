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
    private readonly IProjectReadRepository _projectReadRepository;
    private readonly ICurrentUser _currentUser;

    public AddInvestmentCommandHandler(
        IInvestmentRepository investmentRepository,
        IProjectReadRepository projectReadRepository,
        ICurrentUser currentUser)
    {
        _investmentRepository = investmentRepository;
        _projectReadRepository = projectReadRepository;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(AddInvestmentCommand request, CancellationToken cancellationToken)
    {
        if (request.PlannedDate.HasValue && !request.PlannedAmount.HasValue)
            throw new ArgumentException("Для плановой даты необходимо указать плановую сумму.");
        if (request.ActualDate.HasValue && !request.ActualAmount.HasValue)
            throw new ArgumentException("Для фактической даты необходимо указать фактическую сумму.");

        if (request.ProjectId <= 0)
            throw new ArgumentException("ID проекта должен быть положительным.");

        if (_currentUser.Role != "Investor" && _currentUser.Role != "Admin")
            throw new ForbiddenAccessException("Только инвестор или администратор может добавлять инвестиции.");

        if (!await _projectReadRepository.ExistsAsync(request.ProjectId, cancellationToken))
            throw new NotFoundException("Project", request.ProjectId);

        var status = await _projectReadRepository.GetStatusAsync(request.ProjectId, cancellationToken);
        if (status != "Активен")
            throw new ArgumentException("Инвестиции можно добавлять только для активных проектов.");

        if (request.PlannedDate.HasValue && request.ActualDate.HasValue && request.ActualDate < request.PlannedDate)
            throw new ArgumentException("Фактическая дата не может быть раньше плановой.");

        if (request.PlannedAmount.HasValue && !request.PlannedDate.HasValue)
            throw new ArgumentException("Для плановой суммы необходимо указать плановую дату.");
        if (request.ActualAmount.HasValue && !request.ActualDate.HasValue)
            throw new ArgumentException("Для фактической суммы необходимо указать фактическую дату.");

        var publishedAt = await _projectReadRepository.GetPublishedAtAsync(request.ProjectId, cancellationToken);
        if (request.PlannedDate.HasValue && request.PlannedDate < publishedAt)
            throw new ArgumentException("Плановая дата не может быть раньше даты публикации проекта.");
        if (request.ActualDate.HasValue && request.ActualDate < publishedAt)
            throw new ArgumentException("Фактическая дата не может быть раньше даты публикации проекта.");

        var budget = await _projectReadRepository.GetBudgetAsync(request.ProjectId, cancellationToken) ?? 0;
        var existingInvestments = await _investmentRepository.GetByProjectIdAsync(request.ProjectId, cancellationToken);
        var currentActualSum = existingInvestments.Where(i => i.ActualAmount.HasValue).Sum(i => i.ActualAmount.Value);
        var currentPlannedSum = existingInvestments.Where(i => i.PlannedAmount.HasValue).Sum(i => i.PlannedAmount.Value);

        if (request.ActualAmount.HasValue)
        {
            var newActualSum = currentActualSum + request.ActualAmount.Value;
            if (newActualSum > budget)
                throw new ArgumentException($"Сумма фактических инвестиций превышает бюджет на {newActualSum - budget}.");
        }
        if (request.PlannedAmount.HasValue)
        {
            var newPlannedSum = currentPlannedSum + request.PlannedAmount.Value;
            if (newPlannedSum > budget)
                throw new ArgumentException($"Сумма плановых инвестиций превышает бюджет на {newPlannedSum - budget}.");
        }

        var investment = new Investment(request.ProjectId, request.PlannedAmount, request.PlannedDate, request.ActualAmount, request.ActualDate);
        await _investmentRepository.AddAsync(investment, cancellationToken);
        await _investmentRepository.SaveChangesAsync(cancellationToken);
        return investment.Id;
    }
}