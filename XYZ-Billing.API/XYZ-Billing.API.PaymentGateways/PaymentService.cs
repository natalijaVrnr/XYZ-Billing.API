using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using XYZ.Billing.API.Domain.Models;
using XYZ.Billing.API.PaymentGateways.Dtos;
using XYZ.Billing.API.Persistence;
using XYZ.Billing.API.Cache;
using XYZ.Billing.API.Domain.Enums;

namespace XYZ.Billing.API.PaymentGateways;

public sealed class PaymentService(
    IServiceProvider serviceProvider, 
    IPaymentIdempotencyGuard guard,
    DatabaseContext dbContext) : IPaymentService
{
    public async Task<GatewayPaymentCreationResponse> ProcessPaymentAsync(
        GatewayPaymentCreationRequest paymentDto)
    {
        var gateway = ResolveGateway(paymentDto.GatewayId);

        var existingPayment = await TryGetExistingPaymentAsync(paymentDto.OrderNumber);

        if (existingPayment.Status != PaymentStatus.NotStarted)
        {
            return existingPayment;
        }

        var paymentConfirmation = await gateway.ProcessPaymentAsync(paymentDto);

        var isCreated = await CreatePayment(paymentConfirmation.PaymentId, paymentDto);

        if (!isCreated)
        {
            // TODO - Create custom exception class
            // TODO - How do we handle it here? Payment has been processed, but we failed to create a record in db
            // We would need to retry this multiple times, if we dont succeed, alert the user with error message
            // alert event will be sent to handle this, in the meantime if DB is down return 503 for subsequent requests?
            throw new InvalidOperationException("Failed to create payment record in the database.");
        }

        var isReleased = await guard.ReleasePaymentAsync(paymentDto.OrderNumber);

        if (!isReleased)
        {
            // TODO - Create custom exception class
            throw new InvalidOperationException("Failed to release payment lock.");
        }

        return new GatewayPaymentCreationResponse(PaymentStatus.Succeeded, paymentConfirmation);
    }

    // impatiently waiting for union types in .NET 11 - would be a perfect use case here
    // we could return status only for all payments except succeeded ones, and confirmation dto for succeeded ones
    private async Task<GatewayPaymentCreationResponse> TryGetExistingPaymentAsync(string orderNumber)
    {
        var isClaimed = await guard.TryStartPaymentAsync(
            orderNumber,
            TimeSpan.FromSeconds(CacheConstants.Payment.InProgressTtlSeconds));

        if (!isClaimed)
        {
            // Payment has already been claimed by another process
            return new GatewayPaymentCreationResponse(PaymentStatus.InProgress, null);

        }

        // Check the db to confirm if the payment has already been processed and short circuit if it has
        var existingPayment = await dbContext.Payments
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.OrderNumber == orderNumber);

        if (existingPayment != null)
        {
            // Payment has already been processed
            await guard.ReleasePaymentAsync(orderNumber);

            return new GatewayPaymentCreationResponse(
                PaymentStatus.Succeeded, 
                new PaymentConfirmationResponse(existingPayment.CreatedOn, existingPayment.Id)
            );
        }

        return new GatewayPaymentCreationResponse(PaymentStatus.NotStarted, null);
    }

    // no need to create repo to wrap db context, which is already a unit of work, unless we plan to reuse it in multiple places,
    // for now, we can keep it simple
    private async Task<bool> CreatePayment(Guid paymentId, GatewayPaymentCreationRequest paymentDto)
    {
        var payment = new Payment
        {
            Id = paymentId,
            OrderNumber = paymentDto.OrderNumber,
            UserId = paymentDto.UserId,
            Amount = paymentDto.Amount,
            Currency = paymentDto.Currency,
            GatewayId = paymentDto.GatewayId,
            Description = paymentDto.Description
        };

        dbContext.Payments.Add(payment);
        var result = await dbContext.SaveChangesAsync();
        return result > 0;
    }

    private IPaymentGateway ResolveGateway(PaymentGatewayType gatewayType)
    {
        return serviceProvider.GetRequiredKeyedService<IPaymentGateway>(gatewayType) 
            ?? throw new InvalidOperationException($"No payment gateway found for type: {gatewayType}");
    }
}
