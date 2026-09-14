using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using XYZ.Billing.API.Domain.Enums;
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
    public async Task<PaymentStatus> ProcessPaymentAsync(
        PaymentDto paymentDto,
        CancellationToken cancellationToken = default)
    {
        var gateway = ResolveGateway(paymentDto.GatewayId);

        var paymentStatus = await GetPaymentStatusAsync(paymentDto.OrderNumber, cancellationToken);

        if (paymentStatus == PaymentStatus.NotStarted)
        {
            await gateway.ProcessPaymentAsync(paymentDto, cancellationToken);
            // TODO - write to db
            // TODO - remove order number from cache

        }

        return paymentStatus;
    }

    private async Task<PaymentStatus> GetPaymentStatusAsync(string orderNumber, CancellationToken cancellationToken)
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
            p => p.OrderNumber == orderNumber,
            cancellationToken);

        if (existingPayment != null)
        {
            // Payment has already been processed
            return PaymentStatus.Succeeded;
        }

        return PaymentStatus.NotStarted;
    }

    private IPaymentGateway ResolveGateway(PaymentGatewayType gatewayType)
    {
        return serviceProvider.GetRequiredKeyedService<IPaymentGateway>(gatewayType.ToString()) 
            ?? throw new InvalidOperationException($"No payment gateway found for type: {gatewayType}");
    }
}
