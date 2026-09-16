using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using XYZ.Billing.API.PaymentGateways.Dtos;

namespace XYZ.Billing.API.PaymentGateways.Montonio;

public sealed class MontonioPaymentGateway(IHttpClientFactory factory) : IPaymentGateway
{
    private readonly HttpClient _httpClient = factory.CreateClient(PaymentGatewayConstants.Montonio);
    public async Task<PaymentConfirmationResponse> ProcessPaymentAsync(GatewayPaymentCreationRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("payment", request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PaymentConfirmationResponse>()
            ?? throw new InvalidOperationException(
                $"Empty response returned from Montonio payment gateway. Path: {response.RequestMessage?.RequestUri}");
    }
}
