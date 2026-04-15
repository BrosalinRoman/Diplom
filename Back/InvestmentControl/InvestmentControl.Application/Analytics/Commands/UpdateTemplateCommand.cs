using InvestmentControl.Application.Analytics.DTOs;
using InvestmentControl.Application.Common.Exceptions;
using InvestmentControl.Application.Common.Interfaces;
using InvestmentControl.Domain.Interfaces;
using InvestmentControl.Domain.Models;
using MediatR;
using System.Text.Json;

namespace InvestmentControl.Application.Analytics.Commands;

public class UpdateTemplateCommand : IRequest<TemplateDto>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FiltersJson { get; set; } = string.Empty;
}

public class UpdateTemplateCommandHandler : IRequestHandler<UpdateTemplateCommand, TemplateDto>
{
    private readonly ITemplateRepository _templateRepository;
    private readonly ICurrentUser _currentUser;

    public UpdateTemplateCommandHandler(ITemplateRepository templateRepository, ICurrentUser currentUser)
    {
        _templateRepository = templateRepository;
        _currentUser = currentUser;
    }

    public async Task<TemplateDto> Handle(UpdateTemplateCommand request, CancellationToken cancellationToken)
    {
        if (request.Id <= 0)
            throw new ArgumentException("ID шаблона должен быть положительным.");

        var template = await _templateRepository.GetByIdAsync(request.Id, cancellationToken);
        if (template == null)
            throw new NotFoundException(nameof(Template), request.Id);

        if (template.UserId != _currentUser.UserId)
            throw new ForbiddenAccessException("Вы не можете редактировать чужой шаблон.");

        var existing = await _templateRepository.GetByUserIdAndNameAsync(_currentUser.UserId, request.Name, cancellationToken);
        if (existing != null && existing.Id != template.Id)
            throw new InvalidOperationException("Шаблон с таким именем уже существует.");

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Имя шаблона обязательно.");
        if (request.Name.Length > 100)
            throw new ArgumentException("Имя шаблона не должно превышать 100 символов.");

        if (string.IsNullOrWhiteSpace(request.FiltersJson))
            throw new ArgumentException("FiltersJson обязателен.");
        if (!IsValidFiltersJson(request.FiltersJson))
            throw new ArgumentException("FiltersJson должен быть валидным JSON и содержать поле categoryId.");

        template.Update(request.Name, request.FiltersJson);
        _templateRepository.Update(template);
        await _templateRepository.SaveChangesAsync(cancellationToken);

        return new TemplateDto
        {
            Id = template.Id,
            Name = template.Name,
            CreatedAt = template.CreatedAt,
            UpdatedAt = template.UpdatedAt,
            FiltersJson = template.FiltersJson
        };
    }

    private bool IsValidFiltersJson(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.TryGetProperty("categoryId", out _);
        }
        catch
        {
            return false;
        }
    }
}
