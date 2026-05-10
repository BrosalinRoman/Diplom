using InvestmentControl.Application.Analytics.DTOs;
using InvestmentControl.Application.Common.DTOs;
using InvestmentControl.Application.Common.Helpers;
using InvestmentControl.Application.Common.Interfaces;
using InvestmentControl.Domain.Interfaces;
using MediatR;

namespace InvestmentControl.Application.Analytics.Queries;

public class GetProjectsAnalyticsQuery : IRequest<PagedResponse<ProjectAnalyticsDto>>
{
    public int CategoryId { get; set; }
    public List<int>? DirectionIds { get; set; }
    public List<int>? DepartmentIds { get; set; }
    public List<int>? StatusIds { get; set; }
    public decimal? RankMin { get; set; }
    public decimal? RankMax { get; set; }
    public List<int>? ExcludedProjectIds { get; set; } // ИЗМЕНЕНО: исключаемые ID
    public List<string>? SelectedFields { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class GetProjectsAnalyticsQueryHandler : IRequestHandler<GetProjectsAnalyticsQuery, PagedResponse<ProjectAnalyticsDto>>
{
    private readonly IProjectReadRepository _projectReadRepository;
    private readonly ICurrentUser _currentUser;

    public GetProjectsAnalyticsQueryHandler(IProjectReadRepository projectReadRepository, ICurrentUser currentUser)
    {
        _projectReadRepository = projectReadRepository;
        _currentUser = currentUser;
    }

    public async Task<PagedResponse<ProjectAnalyticsDto>> Handle(GetProjectsAnalyticsQuery request, CancellationToken cancellationToken)
    {
        // Проверка обязательного CategoryId
        if (request.CategoryId <= 0)
            throw new ArgumentException("CategoryId обязателен и должен быть положительным.");

        // Проверка обязательности списков (хотя бы один элемент)
        ValidationHelper.EnsureNonEmptyList(request.DirectionIds, nameof(request.DirectionIds));
        ValidationHelper.EnsureNonEmptyList(request.DepartmentIds, nameof(request.DepartmentIds));
        ValidationHelper.EnsureNonEmptyList(request.StatusIds, nameof(request.StatusIds));

        ValidationHelper.EnsurePositiveIds(request.DirectionIds, nameof(request.DirectionIds));
        ValidationHelper.EnsurePositiveIds(request.DepartmentIds, nameof(request.DepartmentIds));
        ValidationHelper.EnsurePositiveIds(request.StatusIds, nameof(request.StatusIds));
        ValidationHelper.EnsurePositiveIds(request.ExcludedProjectIds, nameof(request.ExcludedProjectIds));


        if (request.RankMin.HasValue && !ValidationHelper.IsValidRankValue(request.RankMin, request.RankMin.ToString()))
            throw new ArgumentException("RankMin может содержать только цифры.");
        if (request.RankMax.HasValue && !ValidationHelper.IsValidRankValue(request.RankMax, request.RankMax.ToString()))
            throw new ArgumentException("RankMax может содержать только цифры.");

        // Проверка рангов от 0 до 100
        if (request.RankMin.HasValue && (request.RankMin < 0 || request.RankMin > 100))
            throw new ArgumentException("RankMin должен быть в диапазоне от 0 до 100.");
        if (request.RankMax.HasValue && (request.RankMax < 0 || request.RankMax > 100))
            throw new ArgumentException("RankMax должен быть в диапазоне от 0 до 100.");
        if (request.RankMin.HasValue && request.RankMax.HasValue && request.RankMin > request.RankMax)
            throw new ArgumentException("RankMin не может быть больше RankMax.");

        List<int>? allowedProjectIds = null;
        if (_currentUser.Role == "Applicant")
            allowedProjectIds = await _projectReadRepository.GetProjectIdsByCreatorAsync(_currentUser.UserId, cancellationToken);

        var pagedResult = await _projectReadRepository.GetFilteredProjectsPagedAsync(
            request.CategoryId,
            request.DirectionIds,
            request.DepartmentIds,
            request.StatusIds,
            request.RankMin,
            request.RankMax,
            allowedProjectIds,
            request.ExcludedProjectIds,
            request.Page,
            request.PageSize,
            cancellationToken);

        var items = pagedResult.Items.Select(p => new ProjectAnalyticsDto
        {
            Id = p.Id,
            Name = p.Name,
            Rank = p.Rank,
            Characteristics = p.Characteristics
                .Where(c => request.SelectedFields == null || request.SelectedFields.Contains(c.Key))
                .ToDictionary(c => c.Key, c => c.Value),
            Category = p.Category?.Name ?? string.Empty,
            Direction = p.Direction?.Name ?? string.Empty,
            Status = p.Status?.Name ?? string.Empty,
            Department = p.Department?.Name ?? string.Empty
        }).ToList();

        return new PagedResponse<ProjectAnalyticsDto>
        {
            Items = items,
            Page = pagedResult.Page,
            PageSize = pagedResult.PageSize,
            TotalCount = pagedResult.TotalCount
        };
    }
}
