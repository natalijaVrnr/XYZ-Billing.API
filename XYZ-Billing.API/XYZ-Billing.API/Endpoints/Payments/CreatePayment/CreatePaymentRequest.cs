using Microsoft.Extensions.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using XYZ.Billing.API.Domain.Enums;

namespace XYZ.Billing.API.Endpoints.Payments.CreatePayment;

// built-in validation for minimal apis does not work with records, so made this to be a class
public class CreatePaymentRequest
{
    [Required]
    public string OrderNumber { get; set; }

    [Required]
    public string UserId { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
    public decimal Amount { get; set; }

    [Required]
    public string Currency { get; set; }

    [Required]
    public string GatewayId { get; set; }
    public string? Description { get; set; }
}
