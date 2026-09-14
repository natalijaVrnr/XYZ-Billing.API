using System;
using System.Collections.Generic;
using System.Text;

namespace XYZ.Billing.API.PaymentGateways.Dtos;

public record PaymentConfirmationDto(
    DateTime Timestamp,
    string PaymentId
);
