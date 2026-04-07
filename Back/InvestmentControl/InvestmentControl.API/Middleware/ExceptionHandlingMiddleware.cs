using System.Net;
using System.Text.Json;
using InvestmentControl.Application.Common.Exceptions;
using InvestmentControl.API.DTOs.Responses;

namespace InvestmentControl.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred.");

        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new ErrorResponse();
        switch (exception)
        {
            case NotFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Error = exception.Message;
                break;
            case ForbiddenAccessException:
                response.StatusCode = (int)HttpStatusCode.Forbidden;
                errorResponse.Error = exception.Message;
                break;
            case UnauthorizedAccessException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Error = exception.Message;
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Error = "Внутренняя ошибка сервера.";
                break;
        }

        var json = JsonSerializer.Serialize(errorResponse);
        await response.WriteAsync(json);
    }
}
