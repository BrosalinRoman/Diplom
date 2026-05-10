using InvestmentControl.Application.NSI.DTOs;
using InvestmentControl.Domain.Interfaces;
using MediatR;

namespace InvestmentControl.Application.NSI.Queries;

public class GetStatusesQuery : IRequest<List<StatusDto>> { }

public class GetStatusesQueryHandler : IRequestHandler<GetStatusesQuery, List<StatusDto>>
{
    private readonly IReferenceDataRepository _repository;
    public GetStatusesQueryHandler(IReferenceDataRepository repository) => _repository = repository;

    public async Task<List<StatusDto>> Handle(GetStatusesQuery request, CancellationToken cancellationToken)
    {
        var statuses = await _repository.GetStatusesAsync(cancellationToken);
        return statuses.Select(s => new StatusDto
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description
        }).ToList();
    }
}