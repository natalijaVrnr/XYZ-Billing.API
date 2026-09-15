using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using XYZ.Billing.API.PaymentGateways.Dtos;

namespace XYZ.Billing.API.PaymentGateways.Stripe;

public sealed class StripePaymentGateway(HttpClient httpClient) : IPaymentGateway
{
    public async Task<PaymentConfirmationResponse> ProcessPaymentAsync(PaymentCreationDto request)
    {
        var response = await httpClient.PostAsJsonAsync("/stripe", request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PaymentConfirmationResponse>()
            ?? throw new InvalidOperationException(
                $"Empty response returned from Stripe payment gateway. Path: {response.RequestMessage?.RequestUri}");
    }
}
