using System.Text.Json;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using XYZ.Billing.API.PaymentGateways.Dtos;

namespace XYZ.Billing.API;

public static class MockSetupExtension
{
    public static void AddMockGateways(this WireMockServer server)
    {
        var paymentResponse = new PaymentConfirmationResponse(
            DateTime.UtcNow,
            Guid.NewGuid().ToString()
        );

        // mocking both to always return the same successful payment response
        server
            .Given(Request.Create().WithPath("/montonio/payment"))
            .RespondWith(
                Response.Create()
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(JsonSerializer.Serialize(paymentResponse))
                    .WithDelay(TimeSpan.FromSeconds(2))
            );

        server
            .Given(Request.Create().WithPath("/stripe/payment"))
            .RespondWith(
                Response.Create()
                    .WithHeader("Content-Type", "application/json")
                    .WithBody(JsonSerializer.Serialize(paymentResponse))
                    .WithDelay(TimeSpan.FromSeconds(2))
            );
    }
}
