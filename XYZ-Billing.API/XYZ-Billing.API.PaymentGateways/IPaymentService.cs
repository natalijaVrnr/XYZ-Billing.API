using System;
using System.Collections.Generic;
using System.Text;
using XYZ.Billing.API.Domain.Models;
using XYZ.Billing.API.PaymentGateways.Dtos;

namespace XYZ.Billing.API.PaymentGateways;

public interface IPaymentService
{
    Task<(PaymentStatus status, PaymentConfirmationDto? confirmationDto)> ProcessPaymentAsync(PaymentDto paymentDto, CancellationToken cancellationToken = default);
}
