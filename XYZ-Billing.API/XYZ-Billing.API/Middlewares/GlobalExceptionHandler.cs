using Microsoft.AspNetCore.Diagnostics;
using ILogger = Serilog.ILogger;

namespace XYZ.Billing.API.Middlewares;

internal class GlobalExceptionHandler(ILogger logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        logger.Error(exception, "An error occurred while processing the request.");

        // TODO - map to response and status code based on exception type
        // TODO - map to ProblemDetails
        httpContext.Response.StatusCode = 500;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(new
        {
            error = "An unexpected error occurred"
        }, cancellationToken);

        return true;
    }
}
