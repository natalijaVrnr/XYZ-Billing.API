using Microsoft.AspNetCore.Diagnostics;

namespace XYZ.Billing.API.Middlewares;

internal class GlobalExceptionHandler : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
