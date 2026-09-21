using System;
using System.Collections.Generic;
using System.Text;

namespace XYZ.Billing.API.Endpoints.Payments.CreatePayment;

internal record CreatePaymentResponse(
    string OrderNumber,
    decimal Amount,
    DateTime Timestamp,
    Guid PaymentId
);