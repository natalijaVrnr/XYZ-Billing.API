using System;
using System.Collections.Generic;
using System.Text;

namespace XYZ.Billing.API.PaymentGateways.Dtos;

public sealed record PaymentRequest(
    string OrderId,
    PaymentGatewayType GatewayId
);