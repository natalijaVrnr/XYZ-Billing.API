using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using XYZ.Billing.API.Domain.Enums;
using XYZ.Billing.API.PaymentGateways.Montonio;
using XYZ.Billing.API.PaymentGateways.Stripe;

namespace XYZ.Billing.API.PaymentGateways;

public static class PaymentGatewayServiceCollectionExtension
{
    public static IServiceCollection AddPaymentGateways(this IServiceCollection services)
    {
        services.AddHttpClient(PaymentGatewayConstants.Montonio, client =>
        {
            client.BaseAddress =
                new Uri("http://localhost:9876/montonio/");
        });

        services.AddHttpClient(PaymentGatewayConstants.Stripe, client =>
        {
            client.BaseAddress =
                new Uri("http://localhost:9876/stripe/");
        });

        services.AddKeyedScoped<IPaymentGateway, MontonioPaymentGateway>(PaymentGatewayType.Montonio);
        services.AddKeyedScoped<IPaymentGateway, StripePaymentGateway>(PaymentGatewayType.Stripe);

        services.AddScoped<IPaymentService, PaymentService>();

        return services;
    }
}
