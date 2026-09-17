using System;
using System.Collections.Generic;
using System.Text;
using XYZ.Billing.API.Domain.Enums;

namespace XYZ.Billing.API.Domain.Models;

public class Payment
{
    public Guid Id { get; set; }
    // passed from FE, will be generated on FE automatically as ORD_{yyMMdd}_{some 6-letter random salt}
    public string OrderNumber { get; set; }
    public string UserId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public PaymentGatewayType GatewayId { get; set; }
    public string? Description { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.InProgress;
    public DateTime CreatedOn { get; set; }
}
