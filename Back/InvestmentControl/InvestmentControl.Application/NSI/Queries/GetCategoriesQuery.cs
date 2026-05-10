using InvestmentControl.Application.NSI.DTOs;
using InvestmentControl.Domain.Interfaces;
using MediatR;

namespace InvestmentControl.Application.NSI.Queries;

/// <summary>
/// Запрос на получение всех категорий проектов.
/// </summary>
public class GetCategoriesQuery : IRequest<List<CategoryDto>>
{
}

/// <summary>
/// Обработчик запроса GetCategoriesQuery.
/// </summary>
public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    private readonly IReferenceDataRepository _repository;

    public GetCategoriesQueryHandler(IReferenceDataRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        // Получаем данные из репозитория (ReadModel)
        var categories = await _repository.GetCategoriesAsync(cancellationToken);

        // Маппим в DTO
        return categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description
        }).ToList();
    }
}