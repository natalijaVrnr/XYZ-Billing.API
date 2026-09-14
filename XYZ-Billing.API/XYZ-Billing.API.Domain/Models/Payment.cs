using System;
using System.Collections.Generic;
using System.Text;
using XYZ.Billing.API.Domain.Enums;

namespace XYZ_Billing.API.Domain.Models;

public class Payment
{
    public string OrderNumber { get; set; }
    public string UserId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public PaymentGatewayType GatewayId { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedOn { get; set; }
}
