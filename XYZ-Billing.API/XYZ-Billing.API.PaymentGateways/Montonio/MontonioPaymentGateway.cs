using System;
using System.Collections.Generic;
using System.Text;
using XYZ.Billing.API.PaymentGateways.Dtos;

namespace XYZ.Billing.API.PaymentGateways.Montonio;

internal sealed class MontonioPaymentGateway : IPaymentGateway
{
    public Task<PaymentStatus> GetPaymentStatusAsync(PaymentRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
