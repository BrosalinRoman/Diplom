using InvestmentControl.Application.Common.Exceptions;
using InvestmentControl.Application.Common.Interfaces;
using InvestmentControl.Domain.Interfaces;
using InvestmentControl.Domain.Models;
using MediatR;

namespace InvestmentControl.Application.Control.Commands;

public class UpdateInvestmentCommand : IRequest
{
    public int Id { get; set; }
    public decimal? PlannedAmount { get; set; }
    public DateTime? PlannedDate { get; set; }
    public decimal? ActualAmount { get; set; }
    public DateTime? ActualDate { get; set; }
}

public class UpdateInvestmentCommandHandler : IRequestHandler<UpdateInvestmentCommand>
{
    private readonly IInvestmentRepository _investmentRepository;
    private readonly IProjectReadRepository _projectReadRepository;
    private readonly ICurrentUser _currentUser;

    public UpdateInvestmentCommandHandler(IInvestmentRepository investmentRepository, IProjectReadRepository projectReadRepository, ICurrentUser currentUser)
    {
        _investmentRepository = investmentRepository;
        _projectReadRepository = projectReadRepository;
        _currentUser = currentUser;
    }

    public async Task Handle(UpdateInvestmentCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.Role != "Investor")
            throw new ForbiddenAccessException("Только инвестор может изменять инвестиции.");

        var investment = await _investmentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (investment == null)
            throw new NotFoundException(nameof(Investment), request.Id);

        if (!await _projectReadRepository.ExistsAsync(investment.ProjectId, cancellationToken))
            throw new NotFoundException("Project", investment.ProjectId);

        // Опционально проверяем статус проекта (можно убрать, если не нужно)
        var status = await _projectReadRepository.GetStatusAsync(investment.ProjectId, cancellationToken);
        if (status != "Активен" && status != "Завершен")
            throw new ArgumentException("Инвестиции можно изменять только для активных или завершённых проектов.");

        investment.Update(request.PlannedAmount, request.PlannedDate, request.ActualAmount, request.ActualDate);
        _investmentRepository.Update(investment);
        await _investmentRepository.SaveChangesAsync(cancellationToken);
    }
}