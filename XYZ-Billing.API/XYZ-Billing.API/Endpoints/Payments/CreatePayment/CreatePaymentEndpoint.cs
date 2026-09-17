using Carter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using XYZ.Billing.API.Domain.Enums;
using XYZ.Billing.API.PaymentGateways;
using XYZ.Billing.API.PaymentGateways.Dtos;

namespace XYZ.Billing.API.Endpoints.Payments.CreatePayment;

public class CreatePaymentEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        // no cancellation token here - we dont want to cancel payment midway through if the client disconnects
        app.MapPost("/api/payments", async (CreatePaymentRequest request, IPaymentService paymentService) =>
        {
            if (!Enum.TryParse<PaymentGatewayType>(request.GatewayId, ignoreCase: true, out var resolvedGatewayType))
            {
                return Results.BadRequest(
                    new ApiResponse(
                        StatusCodes.Status400BadRequest, 
                        $"Invalid payment gateway type: {request.GatewayId}"
                ));
            }

            var paymentDto = new GatewayPaymentCreationRequest(
                request.OrderNumber,
                request.UserId,
                request.Amount,
                request.Currency,
                resolvedGatewayType,
                request.Description);

            var result = await paymentService.ProcessPaymentAsync(paymentDto);

            return result.Status switch
            {
                PaymentStatus.Succeeded => result.Confirmation is not null
                    ? Results.Ok(new CreatePaymentResponse(
                        request.OrderNumber,
                        request.Amount,
                        result.Confirmation.Timestamp,
                        result.Confirmation.PaymentId))
                    : Results.Json(
                        new ApiResponse(StatusCodes.Status502BadGateway, "Payment gateway returned an invalid successful payment response."),
                        statusCode: StatusCodes.Status502BadGateway),

                PaymentStatus.InProgress => Results.Json(
                    new ApiResponse(StatusCodes.Status409Conflict, "Payment already in progress for this order"),
                    statusCode: StatusCodes.Status409Conflict),

                PaymentStatus.Failed => Results.Json(
                    new ApiResponse(StatusCodes.Status502BadGateway, "The gateway failed to process payment"),
                    statusCode: StatusCodes.Status502BadGateway),

                _ => Results.Json(
                    new ApiResponse(StatusCodes.Status500InternalServerError, "An unexpected error occured"),
                    statusCode: StatusCodes.Status500InternalServerError)
            };
        });
    }
}
