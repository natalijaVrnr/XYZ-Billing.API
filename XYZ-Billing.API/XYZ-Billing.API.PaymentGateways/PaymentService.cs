using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using XYZ.Billing.API.Domain.Models;
using XYZ.Billing.API.PaymentGateways.Dtos;
using XYZ.Billing.API.Persistence;
using XYZ_Billing.API.Cache;
using static XYZ_Billing.API.Cache.CacheConstants.Payment;

namespace XYZ.Billing.API.PaymentGateways;

public sealed class PaymentService(
    IServiceProvider serviceProvider, 
    IPaymentIdempotencyGuard guard,
    DatabaseContext dbContext) : IPaymentService
{
    public async Task<(PaymentStatus status, PaymentConfirmationResponse? confirmationDto)> ProcessPaymentAsync(
        PaymentCreationDto paymentDto)
    {
        var gateway = ResolveGateway(paymentDto.GatewayId);

        var paymentStatus = await GetPaymentStatusAsync(paymentDto.OrderNumber);

        if (paymentStatus == PaymentStatus.NotStarted)
        {
            var paymentConfirmation = await gateway.ProcessPaymentAsync(paymentDto);
            // TODO - write to db
            // TODO - remove order number from cache
            return (PaymentStatus.Succeeded, paymentConfirmation);
        }

        return (paymentStatus, null);
    }

    private async Task<PaymentStatus> GetPaymentStatusAsync(string orderNumber)
    {
        var isClaimed = await guard.TryStartPaymentAsync(
            orderNumber,
            TimeSpan.FromSeconds(CacheConstants.Payment.InProgressTtlSeconds));

        if (!isClaimed)
        {
            // Payment has already been claimed by another process
            return PaymentStatus.InProgress;

        }

        // Check the db to confirm if the payment has already been processed and short circuit if it has
        var existingPayment = await dbContext.Payments.SingleOrDefaultAsync(
            p => p.OrderNumber == orderNumber);

        if (existingPayment != null)
        {
            // Payment has already been processed
            return PaymentStatus.Succeeded;
        }

        return PaymentStatus.NotStarted;
    }

    private IPaymentGateway ResolveGateway(string gatewayType)
    {
        return serviceProvider.GetRequiredKeyedService<IPaymentGateway>(gatewayType.ToLowerInvariant()) 
            ?? throw new InvalidOperationException($"No payment gateway found for type: {gatewayType}");
    }
}
