using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using XYZ.Billing.API.PaymentGateways.Montonio;
using XYZ.Billing.API.PaymentGateways.Stripe;

namespace XYZ.Billing.API.PaymentGateways;

public static class PaymentGatewayServiceCollectionExtension
{
    public static IServiceCollection AddPaymentGateways(this IServiceCollection services)
    {
        services.AddKeyedScoped<IPaymentGateway, MontonioPaymentGateway>("montonio");
        services.AddKeyedScoped<IPaymentGateway, StripePaymentGateway>("stripe");

        services.AddScoped<IPaymentService, PaymentService>();

        return services;
    }
}
