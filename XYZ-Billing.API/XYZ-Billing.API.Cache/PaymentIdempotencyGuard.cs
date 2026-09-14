using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace XYZ_Billing.API.Cache;

public class PaymentIdempotencyGuard(IConnectionMultiplexer multiplexer) : IPaymentIdempotencyGuard
{
    private readonly IDatabase _redis = multiplexer.GetDatabase();

    // attempts to claim "in progress" status for order payment
    // returns (isClaimed: true) if caller now owns payment attempt
    // returns (isClaimed: false, cachedStatus) if another process owns it
    public async Task<(bool isClaimed, string? CachedStatus)> TryStartPaymentAsync(
        string orderId,
        TimeSpan ttl)
    {
        var key = CacheConstants.Payment.Key(orderId);

        // only succeeds to set if key does not already exist in Redis
        bool isSet = await _redis.StringSetAsync(key, CacheConstants.Payment.Status.InProgress, ttl, When.NotExists);

        if (isSet)
        {
            // successfully claimed the payment attempt
            return (true, null);
        }

        // get the existing status from cache
        var existing = await _redis.StringGetAsync(key);

        if (existing.IsNullOrEmpty)
        {
            // rare race - key expired between the failed set attempt and this read, will retry to claim the payment attempt
            bool isRetrySet = await _redis.StringSetAsync(key, CacheConstants.Payment.Status.InProgress, ttl, When.NotExists);

            return isRetrySet ? (true, null) : (false, (string?)await _redis.StringGetAsync(key));
        }

        return (false, existing.ToString());
    }

    // record terminal state after payment attempt is complete
    // with a longer TTL so idempotent replays don't do expensive payment gateway call
    public async Task SetPaymentTerminalStatusAsync(
        string orderId,
        string status, // "succeeded" / "failed"
        TimeSpan ttl)
    {
        var key = Key(orderId);
        await _redis.StringSetAsync(key, status, ttl);
    }

    public async Task<string?> GetPaymentStatusAsync(string orderId)
    {
        var key = Key(orderId);
        var value = await _redis.StringGetAsync(key);
        return value.IsNullOrEmpty ? null : value.ToString();
    }
}
