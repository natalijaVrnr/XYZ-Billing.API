using System;
using System.Collections.Generic;
using System.Text;

namespace XYZ.Billing.API.Features.Payments.ProcessPayment;

internal record ProcessPaymentResponse(
    string OrderNumber,
    decimal Amount,
    DateTime Timestamp,
    string PaymentId
);