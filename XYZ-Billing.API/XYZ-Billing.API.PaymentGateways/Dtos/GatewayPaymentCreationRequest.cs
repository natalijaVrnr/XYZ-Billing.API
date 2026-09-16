using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using XYZ.Billing.API.Domain.Enums;

namespace XYZ.Billing.API.PaymentGateways.Dtos;

public record GatewayPaymentCreationRequest(
    string OrderNumber,
    string UserId,
    decimal Amount,
    string Currency,
    PaymentGatewayType GatewayId,
    string? Description = null
);