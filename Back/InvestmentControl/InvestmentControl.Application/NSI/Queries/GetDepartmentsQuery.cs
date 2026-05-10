using InvestmentControl.Application.NSI.DTOs;
using InvestmentControl.Domain.Interfaces;
using MediatR;

namespace InvestmentControl.Application.NSI.Queries;

/// <summary>
/// Запрос на получение всех подразделений (например, НОД-1, НОД-2).
/// </summary>
public class GetDepartmentsQuery : IRequest<List<DepartmentDto>>
{
}

/// <summary>
/// Обработчик запроса GetDepartmentsQuery.
/// </summary>
public class GetDepartmentsQueryHandler : IRequestHandler<GetDepartmentsQuery, List<DepartmentDto>>
{
    private readonly IReferenceDataRepository _repository;

    public GetDepartmentsQueryHandler(IReferenceDataRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<DepartmentDto>> Handle(GetDepartmentsQuery request, CancellationToken cancellationToken)
    {
        var departments = await _repository.GetDepartmentsAsync(cancellationToken);

        return departments.Select(d => new DepartmentDto
        {
            Id = d.Id,
            Name = d.Name,
            Description = d.Description
        }).ToList();
    }
}