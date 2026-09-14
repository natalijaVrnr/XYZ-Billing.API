using System;
using System.Collections.Generic;
using System.Text;
using XYZ.Billing.API.PaymentGateways.Dtos;

namespace XYZ.Billing.API.PaymentGateways;

public abstract class PaymentGateway
{
    public abstract Task<PaymentStatus> GetPaymentStatusFromGatewayAsync(PaymentRequest request, CancellationToken cancellationToken = default);
    public abstract Task<PaymentResult> SendPaymentToGatewayAsync(PaymentRequest request, CancellationToken cancellationToken = default);

    public async Task<PaymentStatus> GetStatusAsync(
        PaymentRequest request,
        CancellationToken cancellationToken = default)
    {
        // Gateway-specific API call
        var status = await GetPaymentStatusFromGatewayAsync(
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
        var status = await GetStatusAsync(request, cancellationToken);

        // TODO - check the status and decide whether to proceed with the payment or return an error

        // Gateway-specific API call
        var result = await SendPaymentToGatewayAsync(
            request,
            cancellationToken);
        return result;
    }

}