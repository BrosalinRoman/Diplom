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

    public AddInvestmentCommandHandler(IInvestmentRepository investmentRepository, IProjectReadRepository projectReadRepository, ICurrentUser currentUser)
    {
        _investmentRepository = investmentRepository;
        _projectReadRepository = projectReadRepository;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(AddInvestmentCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.Role != "Investor")
            throw new ForbiddenAccessException("Только инвестор может добавлять инвестиции.");

        if (!await _projectReadRepository.ExistsAsync(request.ProjectId, cancellationToken))
            throw new NotFoundException("Project", request.ProjectId);

        var status = await _projectReadRepository.GetStatusAsync(request.ProjectId, cancellationToken);
        if (status != "Активен" && status != "Завершен")
            throw new ArgumentException("Инвестиции можно добавлять только для активных или завершённых проектов.");

        var investment = new Investment(request.ProjectId, request.PlannedAmount, request.PlannedDate, request.ActualAmount, request.ActualDate);
        await _investmentRepository.AddAsync(investment, cancellationToken);
        await _investmentRepository.SaveChangesAsync(cancellationToken);
        return investment.Id;
    }
}