using System;
using System.Collections.Generic;
using System.Text;
using XYZ.Billing.API.PaymentGateways.Dtos;

namespace XYZ.Billing.API.PaymentGateways.Montonio;

internal class MontonioPaymentGateway : PaymentGateway
{
    public override Task<PaymentStatus> GetPaymentStatusFromGatewayAsync(PaymentRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public override Task<PaymentResult> SendPaymentToGatewayAsync(PaymentRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
