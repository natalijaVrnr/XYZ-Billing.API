using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using XYZ.Billing.API.Domain.Enums;
using XYZ.Billing.API.PaymentGateways;
using XYZ.Billing.API.PaymentGateways.Dtos;

namespace XYZ.Billing.API.Features.Payments.CreatePayment;

public record CreatePaymentRequest(
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
    PaymentGatewayType GatewayId,
    string ? Description = null
) : PaymentCreationDto(
    OrderNumber,
    UserId,
    Amount,
    Currency,
    GatewayId,
    Description
);
