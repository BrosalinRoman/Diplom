// Application/Control/Queries/GetCostsQuery.cs
using MediatR;
using InvestmentControl.Application.Control.DTOs;
using InvestmentControl.Domain.Interfaces;

namespace InvestmentControl.Application.Control.Queries;

public class GetCostsQuery : IRequest<List<CostDto>>
{
    public int ProjectId { get; set; }
}

public class GetCostsQueryHandler : IRequestHandler<GetCostsQuery, List<CostDto>>
{
    private readonly ICostRepository _costRepository;

    public GetCostsQueryHandler(ICostRepository costRepository)
    {
        _costRepository = costRepository;
    }

    public async Task<List<CostDto>> Handle(GetCostsQuery request, CancellationToken cancellationToken)
    {
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