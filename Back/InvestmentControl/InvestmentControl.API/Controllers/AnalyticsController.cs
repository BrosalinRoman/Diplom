using InvestmentControl.API.DTOs.Requests;
using InvestmentControl.API.DTOs.Responses;
using InvestmentControl.Application.Analytics.Commands;
using InvestmentControl.Application.Analytics.DTOs;
using InvestmentControl.Application.Analytics.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvestmentControl.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AnalyticsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("projects")]
    public async Task<ActionResult<AnalyticsResponse>> GetProjects([FromQuery] AnalyticsRequest request)
    {
        var query = new GetProjectsAnalyticsQuery
        {
            CategoryId = request.CategoryId,
            DirectionIds = request.DirectionIds,
            DepartmentIds = request.DepartmentIds,
            StatusIds = request.StatusIds,
            RankMin = request.RankMin,
            RankMax = request.RankMax,
            ExcludedProjectIds = request.ProjectIds,
            SelectedFields = request.SelectedFields,
            Search = request.Search
        };

        var projects = await _mediator.Send(query);
        return Ok(new AnalyticsResponse { Projects = projects });
    }

    [HttpGet("templates")]
    public async Task<ActionResult<TemplateListResponse>> GetTemplates()
    {
        var templates = await _mediator.Send(new GetTemplatesQuery());
        return Ok(new TemplateListResponse { Templates = templates });
    }

    [HttpGet("templates/{id}")]
    public async Task<ActionResult<TemplateDto>> GetTemplate(int id)
    {
        var template = await _mediator.Send(new GetTemplateByIdQuery { Id = id });
        return Ok(template);
    }

    [HttpPost("templates")]
    public async Task<ActionResult<int>> CreateTemplate([FromBody] SaveTemplateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FiltersJson))
            request.FiltersJson = "{\"categoryId\": 1}";

        var command = new SaveTemplateCommand
        {
            Name = request.Name,
            FiltersJson = request.FiltersJson
        };
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetTemplate), new { id }, id);
    }

    [HttpPut("templates/{id}")]
    public async Task<ActionResult<TemplateDto>> UpdateTemplate(int id, [FromBody] UpdateTemplateRequest request)
    {
        var command = new UpdateTemplateCommand
        {
            Id = id,
            Name = request.Name,
            FiltersJson = request.FiltersJson
        };
        var template = await _mediator.Send(command);
        return Ok(template);
    }

    [HttpDelete("templates/{id}")]
    public async Task<IActionResult> DeleteTemplate(int id)
    {
        await _mediator.Send(new DeleteTemplateCommand { Id = id });
        return NoContent();
    }

    [HttpGet("summary/departments")]
    public async Task<ActionResult<SummaryResponse>> GetSummary([FromQuery] SummaryRequest request)
    {
        var query = new GetSummaryByDepartmentsQuery
        {
            DepartmentIds = request.DepartmentIds,
            DateFrom = request.DateFrom,
            DateTo = request.DateTo,
            StatusIds = request.StatusIds,
            DirectionIds = request.DirectionIds,
            CategoryIds = request.CategoryIds
        };
        var departments = await _mediator.Send(query);

        var response = new SummaryResponse
        {
            Departments = departments,
            TotalBudget = departments.Sum(d => d.TotalBudget),
            TotalProjects = departments.Sum(d => d.ProjectCount)
        };
        return Ok(response);
    }
}