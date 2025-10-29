using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Primitives;

namespace Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ProblemDetailsFactory _problemFactory;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ProblemDetailsFactory problemFactory,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _problemFactory = problemFactory;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var correlationId = EnsureCorrelationId(context);

        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed. CorrelationId: {CorrelationId}", correlationId);

            var pd = _problemFactory.CreateProblemDetails(
                context,
                statusCode: StatusCodes.Status422UnprocessableEntity,
                title: "Validation Failed",
                detail: "One or more validation errors occurred.");

            pd.Extensions["errors"] = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            pd.Extensions["correlationId"] = correlationId;

            await WriteProblemDetails(context, pd);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogInformation(ex, "Invalid operation or resource not found. CorrelationId: {CorrelationId}", correlationId);

            var pd = _problemFactory.CreateProblemDetails(
                context,
                statusCode: StatusCodes.Status404NotFound,
                title: "Resource Not Found / Invalid Operation",
                detail: ex.Message);

            pd.Extensions["correlationId"] = correlationId;

            await WriteProblemDetails(context, pd);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception. CorrelationId: {CorrelationId}", correlationId);

            var pd = _problemFactory.CreateProblemDetails(
                context,
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Unexpected Server Error",
                detail: "An unexpected error occurred.");

            pd.Extensions["correlationId"] = correlationId;

            await WriteProblemDetails(context, pd);
        }
    }

    private static Task WriteProblemDetails(HttpContext context, ProblemDetails pd)
    {
        context.Response.StatusCode = pd.Status ?? StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";
        return context.Response.WriteAsJsonAsync(pd);
    }

    private static string EnsureCorrelationId(HttpContext context)
    {
        const string headerName = "X-Correlation-Id";
        if (!context.Request.Headers.TryGetValue(headerName, out var existing) || StringValues.IsNullOrEmpty(existing))
        {
            var generated = Guid.NewGuid().ToString("N");
            context.Request.Headers[headerName] = generated;
            context.Response.Headers[headerName] = generated;
            return generated;
        }
        else
        {
            context.Response.Headers[headerName] = existing!;
            return existing!;
        }
    }
}