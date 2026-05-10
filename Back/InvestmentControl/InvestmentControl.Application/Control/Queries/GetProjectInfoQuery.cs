using InvestmentControl.Application.Common.Exceptions;
using InvestmentControl.Application.Common.Interfaces;
using InvestmentControl.Application.Control.DTOs;
using InvestmentControl.Domain.Interfaces;
using MediatR;

namespace InvestmentControl.Application.Control.Queries;

public class GetProjectInfoQuery : IRequest<ProjectInfoDto>
{
    public int ProjectId { get; set; }
}

public class GetProjectInfoQueryHandler : IRequestHandler<GetProjectInfoQuery, ProjectInfoDto>
{
    private readonly IProjectReadRepository _projectReadRepository;
    private readonly ICurrentUser _currentUser;

    public GetProjectInfoQueryHandler(IProjectReadRepository projectReadRepository, ICurrentUser currentUser)
    {
        _projectReadRepository = projectReadRepository;
        _currentUser = currentUser;
    }

    public async Task<ProjectInfoDto> Handle(GetProjectInfoQuery request, CancellationToken cancellationToken)
    {
        if (request.ProjectId <= 0)
            throw new ArgumentException("ID проекта должен быть положительным.");

        var project = await _projectReadRepository.GetProjectByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
            throw new NotFoundException("Project", request.ProjectId);

        // Заявитель видит только свои проекты
        if (_currentUser.Role == "Applicant")
        {
            var creatorId = await _projectReadRepository.GetCreatorUserIdAsync(request.ProjectId, cancellationToken);
            if (creatorId != _currentUser.UserId)
                throw new ForbiddenAccessException("Вы не можете просматривать информацию о чужих проектах.");
        }

        return new ProjectInfoDto
        {
            Id = project.Id,
            Name = project.Name,
            Budget = project.Budget ?? 0
        };
    }
}