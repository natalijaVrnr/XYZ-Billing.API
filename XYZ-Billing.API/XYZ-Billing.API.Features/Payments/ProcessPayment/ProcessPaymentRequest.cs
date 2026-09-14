using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYZ.Billing.API.Features.Payments.ProcessPayment;

internal record ProcessPaymentRequest
(
    [Required]
    string OrderNumber,

    [Required]
    string UserId,

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Payable amount must be greater than zero.")]
    decimal PayableAmount,

    [Required]
    string PaymentGatewayId,

    string? Description
);
