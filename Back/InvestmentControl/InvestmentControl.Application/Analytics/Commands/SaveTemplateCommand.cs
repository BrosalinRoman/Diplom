using InvestmentControl.Application.Common.Exceptions;
using InvestmentControl.Application.Common.Interfaces;
using InvestmentControl.Domain.Interfaces;
using InvestmentControl.Domain.Models;
using MediatR;
using System.Text.Json;

namespace InvestmentControl.Application.Analytics.Commands;

public class SaveTemplateCommand : IRequest<int>
{
    public string Name { get; set; } = string.Empty;
    public string FiltersJson { get; set; } = string.Empty;
    public int? TemplateId { get; set; }
}

public class SaveTemplateCommandHandler : IRequestHandler<SaveTemplateCommand, int>
{
    private readonly ITemplateRepository _templateRepository;
    private readonly ICurrentUser _currentUser;

    public SaveTemplateCommandHandler(ITemplateRepository templateRepository, ICurrentUser currentUser)
    {
        _templateRepository = templateRepository;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(SaveTemplateCommand request, CancellationToken cancellationToken)
    {
        // Валидация: проверяем, что FiltersJson содержит categoryId
        if (!IsValidFiltersJson(request.FiltersJson))
            throw new ArgumentException("FiltersJson должен быть валидным JSON и содержать поле categoryId.");

        if (request.TemplateId.HasValue)
        {
            // Обновление существующего
            var template = await _templateRepository.GetByIdAsync(request.TemplateId.Value, cancellationToken);
            if (template == null)
                throw new NotFoundException(nameof(Template), request.TemplateId.Value);

            if (template.UserId != _currentUser.UserId)
                throw new ForbiddenAccessException("Вы не можете редактировать чужой шаблон.");

            // Проверка дубликата имени (исключая текущий шаблон)
            var existing = await _templateRepository.GetByUserIdAndNameAsync(_currentUser.UserId, request.Name, cancellationToken);
            if (existing != null && existing.Id != template.Id)
                throw new InvalidOperationException("Шаблон с таким именем уже существует.");

            template.Update(request.Name, request.FiltersJson);
            _templateRepository.Update(template);
            await _templateRepository.SaveChangesAsync(cancellationToken);
            return template.Id;
        }
        else
        {
            // Создание нового
            // Проверка дубликата имени
            var existing = await _templateRepository.GetByUserIdAndNameAsync(_currentUser.UserId, request.Name, cancellationToken);
            if (existing != null)
                throw new InvalidOperationException("Шаблон с таким именем уже существует.");

            var template = new Template(request.Name, _currentUser.UserId, request.FiltersJson);
            await _templateRepository.AddAsync(template, cancellationToken);
            await _templateRepository.SaveChangesAsync(cancellationToken);
            return template.Id;
        }
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