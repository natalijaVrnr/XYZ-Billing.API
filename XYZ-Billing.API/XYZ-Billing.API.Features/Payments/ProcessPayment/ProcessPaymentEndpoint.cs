using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;
using XYZ.Billing.API.Domain.Models;
using XYZ.Billing.API.PaymentGateways;
using XYZ.Billing.API.PaymentGateways.Dtos;

namespace XYZ.Billing.API.Features.Payments.ProcessPayment;

internal class ProcessPaymentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // we can create a separate request model later if needed, but for now we can use the gateway's PaymentDto model
        app.MapPost("/api/payments/process", async (PaymentDto paymentDto, IPaymentService paymentService) =>
        {
            var result = await paymentService.ProcessPaymentAsync(paymentDto);

            return result.status switch
            {
                PaymentStatus.Succeeded => result.confirmationDto is not null
                    ? Results.Ok(new ProcessPaymentResponse(
                        paymentDto.OrderNumber,
                        paymentDto.Amount,
                        result.confirmationDto.Timestamp,
                        result.confirmationDto.PaymentId))
                    : throw new InvalidOperationException(
                        $"Payment status was Succeeded but no confirmation was returned for order {paymentDto.OrderNumber}."),

                PaymentStatus.InProgress => Results.Conflict(new { message = "Payment already in progress for this order" }),
                PaymentStatus.Failed => Results.Problem("The gateway failed to process payment", statusCode: 502),
                _ => Results.Problem("Unexpected payment status", statusCode: 500)
            };
        });
    }
}
