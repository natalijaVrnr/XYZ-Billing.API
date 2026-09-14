using System;
using System.Collections.Generic;
using System.Text;

namespace XYZ.Billing.API.PaymentGateways.Dtos;

public class PaymentRequest
{
    public PaymentGatewayType GatewayId { get; set; }
}
