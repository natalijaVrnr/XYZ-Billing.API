using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using XYZ.Billing.API.PaymentGateways.Dtos;

namespace XYZ.Billing.API.PaymentGateways;

public sealed class PaymentService(IServiceProvider serviceProvider) : IPaymentService
{
    public async Task<PaymentStatus> GetStatusAsync(
        PaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        var gateway = ResolveGateway(request.GatewayId);

        // Gateway-specific API call
        var status = await gateway.GetPaymentStatusAsync(
            request,
            cancellationToken);

        // TODO - before sending the payment request check cache to see if another payment has already been started
        // TODO - if so, return the status as "Payment already in progress" or similar

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
