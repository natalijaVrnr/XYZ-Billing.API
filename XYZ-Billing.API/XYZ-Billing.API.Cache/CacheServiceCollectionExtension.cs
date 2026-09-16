using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace XYZ.Billing.API.Cache;

public static class CacheServiceCollectionExtension
{
    public static IServiceCollection AddCache(this IServiceCollection services)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = "localhost:8765";
            options.InstanceName = "XYZ:";
        });
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var configuration = ConfigurationOptions.Parse("localhost:8765", true);
            return ConnectionMultiplexer.Connect(configuration);
        });
        services.AddSingleton<IPaymentIdempotencyGuard, PaymentIdempotencyGuard>();

        return services;
    }
}
