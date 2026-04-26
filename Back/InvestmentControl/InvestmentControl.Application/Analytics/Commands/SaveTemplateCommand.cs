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