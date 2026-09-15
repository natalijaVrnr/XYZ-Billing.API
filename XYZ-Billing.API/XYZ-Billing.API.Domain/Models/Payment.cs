using System;
using System.Collections.Generic;
using System.Text;

namespace XYZ_Billing.API.Domain.Models;

public class Payment
{
    public Guid Id { get; set; }
    // passed from FE, will be generated automatically as ORD_{yyMMdd}_{some 6-letter random salt}
    public string OrderNumber { get; set; }
    public string UserId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string GatewayId { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedOn { get; set; }
}
