namespace XYZ_Billing.API.Cache;

public interface IPaymentIdempotencyGuard
{
    Task<bool> TryStartPaymentAsync(string orderId, TimeSpan ttl);
}