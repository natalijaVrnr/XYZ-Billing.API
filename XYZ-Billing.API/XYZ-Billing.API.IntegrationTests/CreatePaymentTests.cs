using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using XYZ.Billing.API.Domain.Enums;
using XYZ.Billing.API.Endpoints.Payments.CreatePayment;

namespace XYZ.Billing.API.IntegrationTests.Tests;

[Collection("AspireApp")]
public class CreatePaymentTests(AspireAppFixture fixture)
{

    [Fact]
    public async Task GetWebResourceRootReturnsOkStatusCode()
    {
        // Act
        using var response = await fixture.ApiClient.GetAsync("/", fixture.CancellationToken);
    
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreatePaymentEndpoint_WhenValidRequestAndPaymentIsNotStarted_Returns200WithConfirmation()
    {
        // Act
        var request = new CreatePaymentRequest
        {
            OrderNumber = "ORD_123454",
            Amount = 100m,
            Currency = "EUR",
            Description = null,
            GatewayId = PaymentGatewayType.Montonio,
            UserId = "TEST_USER_ID"
        };

        using var response = await fixture.ApiClient.PostAsJsonAsync(
            "/api/payments",
            request,
            fixture.CancellationToken);


        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
