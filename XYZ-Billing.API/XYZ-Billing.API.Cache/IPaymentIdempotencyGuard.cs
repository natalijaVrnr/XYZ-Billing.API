namespace XYZ_Billing.API.Cache;

public interface IPaymentIdempotencyGuard
{
    Task<string?> GetPaymentStatusAsync(string orderId);
    Task SetPaymentTerminalStatusAsync(string orderId, string status, TimeSpan ttl);
    Task<(bool isClaimed, string? CachedStatus)> TryStartPaymentAsync(string orderId, TimeSpan ttl);
}