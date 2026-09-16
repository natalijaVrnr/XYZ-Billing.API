using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;
using XYZ.Billing.API.Domain.Enums;
using XYZ.Billing.API.PaymentGateways;
using XYZ.Billing.API.PaymentGateways.Dtos;

namespace XYZ.Billing.API.Features.Payments.CreatePayment;

public class CreatePaymentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // no cancellation token here - we dont want to cancel payment midway through if the client disconnects
        app.MapPost("/api/payments", async (CreatePaymentRequest request, IPaymentService paymentService) =>
        {
            if (!Enum.TryParse<PaymentGatewayType>(request.GatewayId.ToString(), ignoreCase: true, out var resolvedGatewayType))
            {
                return Results.BadRequest(new { message = $"Invalid payment gateway type: {request.GatewayId}" });
            }

            var paymentDto = new PaymentCreationDto(
                request.OrderNumber,
                request.UserId,
                request.Amount,
                request.Currency,
                resolvedGatewayType,
                request.Description);

            var result = await paymentService.ProcessPaymentAsync(paymentDto);

            return result.status switch
            {
                PaymentStatus.Succeeded => result.confirmationDto is not null
                    ? Results.Ok(new CreatePaymentResponse(
                        request.OrderNumber,
                        request.Amount,
                        result.confirmationDto.Timestamp,
                        result.confirmationDto.PaymentId))
                    : throw new InvalidOperationException(
                        $"Payment status was Succeeded but no confirmation was returned for order {request.OrderNumber}."),

                PaymentStatus.InProgress => Results.Conflict(new { message = "Payment already in progress for this order" }),
                PaymentStatus.Failed => Results.Problem("The gateway failed to process payment", statusCode: 502),
                _ => Results.Problem("Unexpected payment status", statusCode: 500)
            };
        });
    }
}
