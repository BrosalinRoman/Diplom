using MediatR;
using InvestmentControl.Application.Analytics.DTOs;
using InvestmentControl.Domain.Interfaces;
using InvestmentControl.Application.Common.Interfaces;

namespace InvestmentControl.Application.Analytics.Queries;

public class GetTemplatesQuery : IRequest<List<TemplateDto>>
{
}

public class GetTemplatesQueryHandler : IRequestHandler<GetTemplatesQuery, List<TemplateDto>>
{
    private readonly ITemplateRepository _templateRepository;
    private readonly ICurrentUser _currentUser;

    public GetTemplatesQueryHandler(ITemplateRepository templateRepository, ICurrentUser currentUser)
    {
        _templateRepository = templateRepository;
        _currentUser = currentUser;
    }

    public async Task<List<TemplateDto>> Handle(GetTemplatesQuery request, CancellationToken cancellationToken)
    {
        var templates = await _templateRepository.GetByUserIdAsync(_currentUser.UserId, cancellationToken);
        return templates.Select(t => new TemplateDto
        {
            Id = t.Id,
            Name = t.Name,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt,
            FiltersJson = t.FiltersJson
        }).ToList();
    }
}
