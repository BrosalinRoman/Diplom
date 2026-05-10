using InvestmentControl.Application.Common.Exceptions;
using InvestmentControl.Application.Common.Interfaces;
using InvestmentControl.Domain.Interfaces;
using InvestmentControl.Domain.Models;
using MediatR;

namespace InvestmentControl.Application.Control.Commands;

public class AddProgressReportCommand : IRequest<int>
{
    public int ProjectId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal ProgressPercentage { get; set; }
}

public class AddProgressReportCommandHandler : IRequestHandler<AddProgressReportCommand, int>
{
    private readonly IProgressReportRepository _progressReportRepository;
    private readonly IProjectReadRepository _projectReadRepository;
    private readonly ICurrentUser _currentUser;

    public AddProgressReportCommandHandler(
        IProgressReportRepository progressReportRepository,
        IProjectReadRepository projectReadRepository,
        ICurrentUser currentUser)
    {
        _progressReportRepository = progressReportRepository;
        _projectReadRepository = projectReadRepository;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(AddProgressReportCommand request, CancellationToken cancellationToken)
    {
        if (request.ProjectId <= 0)
            throw new ArgumentException("ID проекта должен быть положительным.");

        if (_currentUser.Role != "Applicant" && _currentUser.Role != "Admin")
            throw new ForbiddenAccessException("Только заявитель или администратор может добавлять отчёты.");

        if (!await _projectReadRepository.ExistsAsync(request.ProjectId, cancellationToken))
            throw new NotFoundException("Project", request.ProjectId);

        if (_currentUser.Role == "Applicant")
        {
            var creatorId = await _projectReadRepository.GetCreatorUserIdAsync(request.ProjectId, cancellationToken);
            if (creatorId != _currentUser.UserId)
                throw new ForbiddenAccessException("Вы можете добавлять отчёты только для своих проектов.");
        }

        var status = await _projectReadRepository.GetStatusAsync(request.ProjectId, cancellationToken);
        if (status != "Активен" && status != "Завершен")
            throw new ArgumentException("Затраты/отчёты можно добавлять только для активных или завершённых проектов.");

        // Проверка прогресса
        var existingReports = await _progressReportRepository.GetByProjectIdAsync(request.ProjectId, cancellationToken);

        // Проверка на создание отчёта не чаще раза в неделю
        var lastReport = existingReports.OrderByDescending(r => r.ReportDate).FirstOrDefault();
        if (lastReport != null)
        {
            var timeSinceLastReport = DateTime.UtcNow - lastReport.ReportDate;
            if (timeSinceLastReport.TotalDays < 7)
            {
                var remaining = TimeSpan.FromDays(7) - timeSinceLastReport;
                var message = $"Отчеты можно добавлять только раз в неделю. Следующий отчёт вы сможете внести через {remaining.Days} д. {remaining.Hours} ч. {remaining.Minutes} мин.";
                throw new ArgumentException(message);
            }
        }

        var maxProgress = existingReports.Any() ? existingReports.Max(r => r.ProgressPercentage) : 0;
        if (request.ProgressPercentage <= maxProgress)
            throw new ArgumentException($"Новый прогресс ({request.ProgressPercentage}%) должен быть больше предыдущего ({maxProgress}%).");

        var report = new ProgressReport(request.ProjectId, request.Description, request.ProgressPercentage);
        await _progressReportRepository.AddAsync(report, cancellationToken);
        await _progressReportRepository.SaveChangesAsync(cancellationToken);
        return report.Id;
    }
}