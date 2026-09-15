using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYZ.Billing.API.PaymentGateways.Dtos;

public record PaymentCreationDto(
    string OrderNumber,
    string UserId,
    decimal Amount,
    string Currency,
    string GatewayId,
    string? Description = null
);