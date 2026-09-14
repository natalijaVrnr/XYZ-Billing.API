using System;
using System.Collections.Generic;
using System.Text;

namespace XYZ.Billing.API.Domain.Models;

public enum PaymentStatus
{
    NotStarted,
    InProgress,
    Succeeded,
    Failed
}
