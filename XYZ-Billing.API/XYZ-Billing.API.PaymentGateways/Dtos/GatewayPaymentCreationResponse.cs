using System;
using System.Collections.Generic;
using System.Text;
using XYZ.Billing.API.Domain.Enums;

namespace XYZ.Billing.API.PaymentGateways.Dtos;

public record GatewayPaymentCreationResponse(PaymentStatus Status, PaymentConfirmationResponse? Confirmation);
