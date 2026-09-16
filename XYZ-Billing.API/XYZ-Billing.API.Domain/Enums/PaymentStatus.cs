using System;
using System.Collections.Generic;
using System.Text;

namespace XYZ.Billing.API.Domain.Enums;

public enum PaymentStatus
{
    NotStarted,
    InProgress,
    Succeeded,
    Failed
}
