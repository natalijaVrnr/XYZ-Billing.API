using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace XYZ_Billing.API.Cache;

public static class CacheServiceCollectionExtension
{
    public static IServiceCollection AddCache(this IServiceCollection services)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = "localhost:8765";
            options.InstanceName = "XYZ:";
        });

        return services;
    }
}
