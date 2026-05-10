using InvestmentControl.Application.Common.Exceptions;
using InvestmentControl.Application.Common.Interfaces;
using InvestmentControl.Application.Control.DTOs;
using InvestmentControl.Domain.Interfaces;
using InvestmentControl.Domain.Models;
using MediatR;

namespace InvestmentControl.Application.Control.Commands;

public class UpdateProgressReportCommand : IRequest<ProgressReportDto>
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal ProgressPercentage { get; set; }
}

public class UpdateProgressReportCommandHandler : IRequestHandler<UpdateProgressReportCommand, ProgressReportDto>
{
    private readonly IProgressReportRepository _progressReportRepository;
    private readonly IProjectReadRepository _projectReadRepository;
    private readonly ICurrentUser _currentUser;

    public UpdateProgressReportCommandHandler(
        IProgressReportRepository progressReportRepository,
        IProjectReadRepository projectReadRepository,
        ICurrentUser currentUser)
    {
        _progressReportRepository = progressReportRepository;
        _projectReadRepository = projectReadRepository;
        _currentUser = currentUser;
    }

    public async Task<ProgressReportDto> Handle(UpdateProgressReportCommand request, CancellationToken cancellationToken)
    {
        if (request.Id <= 0)
            throw new ArgumentException("ID отчёта должен быть положительным.");

        if (_currentUser.Role != "Applicant" && _currentUser.Role != "Admin")
            throw new ForbiddenAccessException("Только заявитель или администратор может изменять отчёты.");

        var report = await _progressReportRepository.GetByIdAsync(request.Id, cancellationToken);
        if (report == null)
            throw new NotFoundException("ProgressReport", request.Id);

        if (_currentUser.Role == "Applicant")
        {
            var creatorId = await _projectReadRepository.GetCreatorUserIdAsync(report.ProjectId, cancellationToken);
            if (creatorId != _currentUser.UserId)
                throw new ForbiddenAccessException("Вы можете изменять отчёты только для своих проектов.");
        }

        var status = await _projectReadRepository.GetStatusAsync(report.ProjectId, cancellationToken);
        if (status != "Активен" && status != "Завершен")
            throw new ArgumentException("Отчёты можно изменять только для активных или завершённых проектов.");

        // Проверка монотонности прогресса относительно предыдущих и последующих отчётов
        var allReports = (await _progressReportRepository.GetByProjectIdAsync(report.ProjectId, cancellationToken)).ToList();
        var previousReports = allReports.Where(r => r.ReportDate < report.ReportDate).ToList();
        if (previousReports.Any())
        {
            var maxPrevious = previousReports.Max(r => r.ProgressPercentage);
            if (request.ProgressPercentage <= maxPrevious)
                throw new ArgumentException($"Новый прогресс ({request.ProgressPercentage}%) должен быть больше предыдущего ({maxPrevious}%).");
        }

        var laterReports = allReports.Where(r => r.ReportDate > report.ReportDate).ToList();
        if (laterReports.Any())
        {
            var minLater = laterReports.Min(r => r.ProgressPercentage);
            if (request.ProgressPercentage > minLater)
                throw new ArgumentException($"Новый прогресс ({request.ProgressPercentage}%) не может быть больше прогресса в более поздних отчётах ({minLater}%).");
        }

        report.Update(request.Description, request.ProgressPercentage);
        _progressReportRepository.Update(report);
        await _progressReportRepository.SaveChangesAsync(cancellationToken);

        return new ProgressReportDto
        {
            Id = report.Id,
            Description = report.Description,
            ProgressPercentage = report.ProgressPercentage,
            ReportDate = report.UpdatedAt ?? report.ReportDate // возвращаем дату обновления, если есть
        };
    }
}