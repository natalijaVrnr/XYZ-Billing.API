using Carter;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace XYZ.Billing.API.Features.Payments.ProcessPayment;

internal class ProcessPaymentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // we can create a separate request model later if needed, but for now we can use the gateway's PaymentDto model

    }
}
