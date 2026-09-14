using System;
using System.Collections.Generic;
using System.Text;
using XYZ.Billing.API.PaymentGateways.Dtos;

namespace XYZ.Billing.API.PaymentGateways.Stripe;

internal sealed class StripePaymentGateway : IPaymentGateway
{
    public async Task ProcessPaymentAsync(PaymentDto request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
