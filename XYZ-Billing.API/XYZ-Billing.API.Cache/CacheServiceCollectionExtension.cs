using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace XYZ.Billing.API.Cache;

public static class CacheServiceCollectionExtension
{
    public static IServiceCollection AddCacheServices(this IServiceCollection services)
    {
        services.AddSingleton<IPaymentIdempotencyGuard, PaymentIdempotencyGuard>();

        return services;
    }
}
