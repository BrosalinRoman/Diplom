using InvestmentControl.Application.Common.Exceptions;
using InvestmentControl.Application.Common.Interfaces;
using InvestmentControl.Domain.Interfaces;
using InvestmentControl.Domain.Models;
using MediatR;

namespace InvestmentControl.Application.Analytics.Commands;

public class DeleteTemplateCommand : IRequest
{
    public int Id { get; set; }
}

public class DeleteTemplateCommandHandler : IRequestHandler<DeleteTemplateCommand>
{
    private readonly ITemplateRepository _templateRepository;
    private readonly ICurrentUser _currentUser;

    public DeleteTemplateCommandHandler(ITemplateRepository templateRepository, ICurrentUser currentUser)
    {
        _templateRepository = templateRepository;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteTemplateCommand request, CancellationToken cancellationToken)
    {
        var template = await _templateRepository.GetByIdAsync(request.Id, cancellationToken);
        if (template == null)
            throw new NotFoundException(nameof(Template), request.Id);

        if (template.UserId != _currentUser.UserId)
            throw new ForbiddenAccessException("Вы не можете удалить чужой шаблон.");

        _templateRepository.Delete(template);
        await _templateRepository.SaveChangesAsync(cancellationToken);
    }
}
