using MediatR;
using InvestmentControl.Application.Control.DTOs;
using InvestmentControl.Domain.Interfaces;

namespace InvestmentControl.Application.Control.Queries;

public class GetInvestmentsQuery : IRequest<List<InvestmentDto>>
{
    public int ProjectId { get; set; }
}

public class GetInvestmentsQueryHandler : IRequestHandler<GetInvestmentsQuery, List<InvestmentDto>>
{
    private readonly IInvestmentRepository _investmentRepository;

    public GetInvestmentsQueryHandler(IInvestmentRepository investmentRepository)
    {
        _investmentRepository = investmentRepository;
    }

    public async Task<List<InvestmentDto>> Handle(GetInvestmentsQuery request, CancellationToken cancellationToken)
    {
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