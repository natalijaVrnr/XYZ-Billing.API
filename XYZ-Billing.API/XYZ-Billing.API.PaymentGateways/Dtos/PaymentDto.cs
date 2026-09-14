using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using XYZ.Billing.API.Domain.Enums;

namespace XYZ.Billing.API.PaymentGateways.Dtos;

public record PaymentDto(
    [Required]
    string OrderNumber,
    [Required]
    string UserId,
    [Required]
    decimal Amount,
    [Required]
    string Currency,
    [Required]
    PaymentGatewayType GatewayId,
    string? Description = null
);