using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace XYZ_Billing.API.Cache;

public class PaymentIdempotencyGuard(IConnectionMultiplexer multiplexer) : IPaymentIdempotencyGuard
{
    private readonly IDatabase _redis = multiplexer.GetDatabase();

    // attempts to claim the order payment
    // returns true if caller now owns payment attempt
    // returns false if another process owns it
    public async Task<bool> TryStartPaymentAsync(
        string orderId,
        TimeSpan ttl)
    {
        var key = CacheConstants.Payment.Key(orderId);

        // only succeeds to set if key does not already exist in Redis
        bool isSet = await _redis.StringSetAsync(key, CacheConstants.Payment.Status.InProgress, ttl, When.NotExists);

        if (isSet)
        {
            // successfully claimed the payment attempt
            return true;
        }

        // get the existing status from cache
        var existing = await _redis.StringGetAsync(key);

        return existing.IsNullOrEmpty;
    }
}
