using System;
using System.Collections.Generic;
using System.Text;
using XYZ.Billing.API.PaymentGateways.Dtos;

namespace XYZ.Billing.API.PaymentGateways;

public interface IPaymentGateway
{
    Task<PaymentConfirmationResponse> ProcessPaymentAsync(PaymentCreationDto request);
}