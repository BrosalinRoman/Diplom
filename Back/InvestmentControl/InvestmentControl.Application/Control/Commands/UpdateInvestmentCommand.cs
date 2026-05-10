using InvestmentControl.Application.Common.Exceptions;
using InvestmentControl.Application.Common.Interfaces;
using InvestmentControl.Application.Control.DTOs;
using InvestmentControl.Domain.Interfaces;
using InvestmentControl.Domain.Models;
using MediatR;

namespace InvestmentControl.Application.Control.Commands;

public class UpdateInvestmentCommand : IRequest<InvestmentDto>
{
    public int Id { get; set; }
    public decimal? PlannedAmount { get; set; }
    public DateTime? PlannedDate { get; set; }
    public decimal? ActualAmount { get; set; }
    public DateTime? ActualDate { get; set; }
}

public class UpdateInvestmentCommandHandler : IRequestHandler<UpdateInvestmentCommand, InvestmentDto>
{
    private readonly IInvestmentRepository _investmentRepository;
    private readonly IProjectReadRepository _projectReadRepository;
    private readonly ICurrentUser _currentUser;

    public UpdateInvestmentCommandHandler(
        IInvestmentRepository investmentRepository,
        IProjectReadRepository projectReadRepository,
        ICurrentUser currentUser)
    {
        _investmentRepository = investmentRepository;
        _projectReadRepository = projectReadRepository;
        _currentUser = currentUser;
    }

    public async Task<InvestmentDto> Handle(UpdateInvestmentCommand request, CancellationToken cancellationToken)
    {
        if (request.Id <= 0)
            throw new ArgumentException("ID инвестиции должен быть положительным.");

        if (_currentUser.Role != "Investor" && _currentUser.Role != "Admin")
            throw new ForbiddenAccessException("Только инвестор или администратор может изменять инвестиции.");

        var investment = await _investmentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (investment == null)
            throw new NotFoundException("Investment", request.Id);

        if (!await _projectReadRepository.ExistsAsync(investment.ProjectId, cancellationToken))
            throw new NotFoundException("Project", investment.ProjectId);

        var status = await _projectReadRepository.GetStatusAsync(investment.ProjectId, cancellationToken);
        if (status != "Активен")
            throw new ArgumentException("Инвестиции можно изменять только для активных проектов.");

        // Проверки дат
        var publishedAt = await _projectReadRepository.GetPublishedAtAsync(investment.ProjectId, cancellationToken);
        if (request.PlannedDate.HasValue && request.PlannedDate < publishedAt)
            throw new ArgumentException("Плановая дата не может быть раньше даты публикации проекта.");
        if (request.ActualDate.HasValue && request.ActualDate < publishedAt)
            throw new ArgumentException("Фактическая дата не может быть раньше даты публикации проекта.");

        if (!request.PlannedDate.HasValue && request.ActualDate.HasValue)
        {
            var lastPlannedDate = await _investmentRepository.GetLastPlannedDateAsync(investment.ProjectId, request.Id, cancellationToken);
            if (lastPlannedDate.HasValue && request.ActualDate <= lastPlannedDate.Value)
                throw new ArgumentException($"Фактическая дата не может быть равна или раньше последней плановой даты инвестиций ({lastPlannedDate.Value:d}).");
        }

        // Бюджет с учётом других инвестиций
        var budget = await _projectReadRepository.GetBudgetAsync(investment.ProjectId, cancellationToken) ?? 0;
        var allInvestments = await _investmentRepository.GetByProjectIdAsync(investment.ProjectId, cancellationToken);
        var otherInvestments = allInvestments.Where(i => i.Id != request.Id).ToList();

        var currentActualSum = otherInvestments.Where(i => i.ActualAmount.HasValue).Sum(i => i.ActualAmount.Value);
        var currentPlannedSum = otherInvestments.Where(i => i.PlannedAmount.HasValue).Sum(i => i.PlannedAmount.Value);

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

        investment.Update(request.PlannedAmount, request.PlannedDate, request.ActualAmount, request.ActualDate);
        _investmentRepository.Update(investment);
        await _investmentRepository.SaveChangesAsync(cancellationToken);

        return new InvestmentDto
        {
            Id = investment.Id,
            PlannedAmount = investment.PlannedAmount,
            PlannedDate = investment.PlannedDate,
            ActualAmount = investment.ActualAmount,
            ActualDate = investment.ActualDate
        };
    }
}