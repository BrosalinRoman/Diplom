using InvestmentControl.Application.Analytics.DTOs;
using InvestmentControl.Application.Common.Exceptions;
using InvestmentControl.Application.Common.Interfaces;
using InvestmentControl.Domain.Interfaces;
using InvestmentControl.Domain.Models;
using MediatR;

namespace InvestmentControl.Application.Analytics.Queries;

public class GetTemplateByIdQuery : IRequest<TemplateDto>
{
    public int Id { get; set; }
}

public class GetTemplateByIdQueryHandler : IRequestHandler<GetTemplateByIdQuery, TemplateDto>
{
    private readonly ITemplateRepository _templateRepository;
    private readonly ICurrentUser _currentUser;

    public GetTemplateByIdQueryHandler(ITemplateRepository templateRepository, ICurrentUser currentUser)
    {
        _templateRepository = templateRepository;
        _currentUser = currentUser;
    }

    public async Task<TemplateDto> Handle(GetTemplateByIdQuery request, CancellationToken cancellationToken)
    {
        var template = await _templateRepository.GetByIdAsync(request.Id, cancellationToken);
        if (template == null)
            throw new NotFoundException(nameof(Template), request.Id);

        // Проверяем, что шаблон принадлежит текущему пользователю (или администратору?)
        if (template.UserId != _currentUser.UserId && _currentUser.Role != "Admin")
            throw new ForbiddenAccessException("Нет доступа к этому шаблону.");

        return new TemplateDto
        {
            Id = template.Id,
            Name = template.Name,
            CreatedAt = template.CreatedAt,
            UpdatedAt = template.UpdatedAt,
            FiltersJson = template.FiltersJson
        };
    }
}
