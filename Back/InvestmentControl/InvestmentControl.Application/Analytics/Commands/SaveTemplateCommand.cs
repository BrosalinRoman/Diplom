using InvestmentControl.Application.Common.Exceptions;
using InvestmentControl.Application.Common.Interfaces;
using InvestmentControl.Domain.Interfaces;
using InvestmentControl.Domain.Models;
using MediatR;

namespace InvestmentControl.Application.Analytics.Commands;

public class SaveTemplateCommand : IRequest<int>
{
    public string Name { get; set; } = string.Empty;
    public string FiltersJson { get; set; } = string.Empty;
    public int? TemplateId { get; set; } // если указан, то обновление, иначе создание
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
        if (request.TemplateId.HasValue)
        {
            // Обновление существующего
            var template = await _templateRepository.GetByIdAsync(request.TemplateId.Value, cancellationToken);
            if (template == null)
                throw new NotFoundException(nameof(Template), request.TemplateId.Value);

            // Проверка, что шаблон принадлежит текущему пользователю
            if (template.UserId != _currentUser.UserId)
                throw new ForbiddenAccessException("Вы не можете редактировать чужой шаблон.");

            template.Update(request.Name, request.FiltersJson);
            _templateRepository.Update(template);
        }
        else
        {
            // Создание нового
            var template = new Template(request.Name, _currentUser.UserId, request.FiltersJson);
            await _templateRepository.AddAsync(template, cancellationToken);
        }

        await _templateRepository.SaveChangesAsync(cancellationToken);
        return request.TemplateId ?? 0; // возвращаем ID (для созданного можно вернуть из AddAsync)
    }
}
