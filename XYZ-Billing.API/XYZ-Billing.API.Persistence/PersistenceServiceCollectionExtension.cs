using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace XYZ.Billing.API.Persistence;

public static class PersistenceServiceCollectionExtension
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        // no migrations needed for in-memory db
        services.AddDbContext<DatabaseContext>(options =>
        {
            options.UseInMemoryDatabase("InMemoryDb");
        });

        return services;
    }
}
