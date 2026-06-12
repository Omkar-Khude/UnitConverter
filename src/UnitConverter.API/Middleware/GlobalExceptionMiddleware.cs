using System.Net;
using System.Text.Json;
using UnitConverter.API.Models;
using UnitConverter.API.Services;

namespace UnitConverter.API.Middleware;

/// <summary>
/// Catches unhandled exceptions and returns a consistent JSON error envelope.
/// </summary>
public sealed class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteErrorAsync(context, ex);
        }
    }

    private static Task WriteErrorAsync(HttpContext context, Exception ex)
    {
        var (status, error) = ex switch
        {
            UnsupportedUnitException    => (HttpStatusCode.BadRequest,           "UnsupportedUnit"),
            IncompatibleUnitsException  => (HttpStatusCode.UnprocessableEntity,  "IncompatibleUnits"),
            ArgumentException           => (HttpStatusCode.BadRequest,           "InvalidArgument"),
            _                           => (HttpStatusCode.InternalServerError,  "InternalServerError")
        };

        context.Response.StatusCode  = (int)status;
        context.Response.ContentType = "application/json";

        var body = JsonSerializer.Serialize(new ErrorResponse
        {
            Error      = error,
            Message    = ex.Message,
            StatusCode = (int)status
        }, JsonOptions);

        return context.Response.WriteAsync(body);
    }
}
