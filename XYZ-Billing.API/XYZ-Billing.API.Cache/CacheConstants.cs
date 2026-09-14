using System;
using System.Collections.Generic;
using System.Text;

namespace XYZ_Billing.API.Cache;

public static class CacheConstants
{
    public static class Payment
    {
        public static string Key(string orderId) => $"payment:{orderId}";
        public static int InProgressTtlSeconds => 30;
        public static class Status
        {
            public const string InProgress = "in_progress";
            public const string Succeeded = "succeeded";
            public const string Failed = "failed";
        }
    }
    
}
