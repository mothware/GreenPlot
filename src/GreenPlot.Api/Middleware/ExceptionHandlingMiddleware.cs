using GreenPlot.Domain.Exceptions;
using System.Text.Json;

namespace GreenPlot.Api.Middleware;

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
        catch (NotFoundException ex)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await WriteJson(context, new { error = ex.Message });
        }
        catch (ForbiddenException)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await WriteJson(context, new { error = "Access forbidden." });
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            await WriteJson(context, new { errors = ex.Errors });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await WriteJson(context, new { error = "An unexpected error occurred." });
        }
    }

    private static Task WriteJson(HttpContext context, object body)
    {
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsync(JsonSerializer.Serialize(body));
    }
}
