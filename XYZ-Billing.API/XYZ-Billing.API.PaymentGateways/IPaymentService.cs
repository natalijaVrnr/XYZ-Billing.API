using System;
using System.Collections.Generic;
using System.Text;
using XYZ.Billing.API.Domain.Enums;
using XYZ.Billing.API.PaymentGateways.Dtos;

namespace XYZ.Billing.API.PaymentGateways;

public interface IPaymentService
{
    Task<GatewayPaymentCreationResponse> ProcessPaymentAsync(GatewayPaymentCreationRequest paymentDto);
}
