using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using XYZ.Billing.API.PaymentGateways.Dtos;
using XYZ_Billing.API.Cache;

namespace XYZ.Billing.API.PaymentGateways;

public sealed class PaymentService(IServiceProvider serviceProvider, IPaymentIdempotencyGuard guard) : IPaymentService
{
    public async Task<PaymentStatus> GetStatusAsync(
        PaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        var gateway = ResolveGateway(request.GatewayId);

        var (isClaimed, cachedStatus) = await guard.TryStartPaymentAsync(
            request.OrderId, 
            TimeSpan.FromSeconds(CacheConstants.Payment.InProgressTtlSeconds));

        if (isClaimed)
        {
            // TODO - create a custom exception type for this scenario
            throw new Exception("Payment is already in progress for this order.");
        }

        // Gateway-specific API call
        var status = await gateway.GetPaymentStatusAsync(
            request,
            cancellationToken);

        // TODO - add checks on status retrieved from gateway
        // TODO - cache the status for future requests

        return status;
    }

    public async Task<PaymentResult> ProcessPaymentAsync(
        PaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        var gateway = ResolveGateway(request.GatewayId);

        var status = await gateway.GetPaymentStatusAsync(request, cancellationToken);

        // TODO - check the status and decide whether to proceed with the payment or return an error

        // Gateway-specific API call
        var result = await gateway.ProcessPaymentAsync(
            request,
            cancellationToken);

        return result;
    }

    private IPaymentGateway ResolveGateway(PaymentGatewayType gatewayType)
    {
        return serviceProvider.GetRequiredKeyedService<IPaymentGateway>(gatewayType.ToString()) 
            ?? throw new InvalidOperationException($"No payment gateway found for type: {gatewayType}");
    }
}
