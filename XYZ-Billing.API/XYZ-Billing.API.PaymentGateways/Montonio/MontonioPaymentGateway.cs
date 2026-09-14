using System;
using System.Collections.Generic;
using System.Text;
using XYZ.Billing.API.PaymentGateways.Dtos;

namespace XYZ.Billing.API.PaymentGateways.Montonio;

internal sealed class MontonioPaymentGateway : IPaymentGateway
{
    public async Task ProcessPaymentAsync(PaymentDto request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
