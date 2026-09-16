namespace XYZ.Billing.API.Cache;

public interface IPaymentIdempotencyGuard
{
    Task<bool> ReleasePaymentAsync(string orderId);
    Task<bool> TryStartPaymentAsync(string orderId, TimeSpan ttl);
}