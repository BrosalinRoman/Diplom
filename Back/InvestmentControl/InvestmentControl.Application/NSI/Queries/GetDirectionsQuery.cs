using InvestmentControl.Application.NSI.DTOs;
using InvestmentControl.Domain.Interfaces;
using MediatR;

namespace InvestmentControl.Application.NSI.Queries;

/// <summary>
/// Запрос на получение всех направлений (например, "Капитальное строительство").
/// </summary>
public class GetDirectionsQuery : IRequest<List<DirectionDto>>
{
}

/// <summary>
/// Обработчик запроса GetDirectionsQuery.
/// </summary>
public class GetDirectionsQueryHandler : IRequestHandler<GetDirectionsQuery, List<DirectionDto>>
{
    private readonly IReferenceDataRepository _repository;

    public GetDirectionsQueryHandler(IReferenceDataRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<DirectionDto>> Handle(GetDirectionsQuery request, CancellationToken cancellationToken)
    {
        var directions = await _repository.GetDirectionsAsync(cancellationToken);

        return directions.Select(d => new DirectionDto
        {
            Id = d.Id,
            Name = d.Name,
            Description = d.Description
        }).ToList();
    }
}