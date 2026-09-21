using System.Text;
using System.Text.Json;
using WireMock;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Types;
using WireMock.Util;
using XYZ.Billing.API.PaymentGateways.Dtos;

namespace XYZ.Billing.API;

public static class MockSetupExtension
{
    public static void AddMockGateways(this WireMockServer server)
    {
        // mocking both to always return the same successful payment response
        server
            .Given(Request.Create().WithPath("/montonio/payment"))
            .RespondWith(CreateSuccessfulResponse());

        server
            .Given(Request.Create().WithPath("/stripe/payment"))
            .RespondWith(CreateSuccessfulResponse());
    }

    private static IResponseBuilder CreateSuccessfulResponse()
    {
        return Response.Create()
            .WithCallback(_ =>
            {
                var paymentResponse = new PaymentConfirmationResponse(
                    DateTime.UtcNow,
                    Guid.NewGuid());

                return new ResponseMessage
                {
                    StatusCode = 200,
                    Headers = new Dictionary<string, WireMockList<string>>
                    {
                        ["Content-Type"] = new WireMockList<string>("application/json")
                    },
                    BodyData = new BodyData
                    {
                        BodyAsString = JsonSerializer.Serialize(paymentResponse),
                        DetectedBodyType = BodyType.String,
                        Encoding = Encoding.UTF8
                    }
                };
            })
            .WithDelay(TimeSpan.FromSeconds(2));
    }
}
