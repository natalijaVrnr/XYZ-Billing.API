using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using XYZ.Billing.API.PaymentGateways.Dtos;

namespace XYZ.Billing.API.Features.Payments.ProcessPayment;

public record ProcessPaymentRequest(
    [Required]
    string OrderNumber,
    [Required]
    string UserId,
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
    decimal Amount,
    [Required]
    string Currency,
    [Required]
    string GatewayId,
    string ? Description = null
) : PaymentCreationDto(
    OrderNumber,
    UserId,
    Amount,
    Currency,
    GatewayId,
    Description
);
