using JsonConverter.System.Text.Json.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using XYZ.Billing.API.Domain.Enums;
using XYZ.Billing.API.Endpoints.Payments.CreatePayment;

namespace XYZ.Billing.API.IntegrationTests.Tests;

[Collection("AspireApp")]
public class CreatePaymentTests(AspireAppFixture fixture)
{

    [Fact]
    public async Task HealthEndpointReturnsOkStatusCode()
    {
        // Act
        using var response = await fixture.ApiClient.GetAsync("/health", fixture.CancellationToken);
    
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
            GatewayId = "Montonio",
            UserId = "TEST_USER_ID"
        };

        using var response = await fixture.ApiClient.PostAsJsonAsync(
            "/api/payments",
            request,
            fixture.CancellationToken);


        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreatePaymentEndpoint_WhenOrderNumberEmpty_Returns400WithValidationProblemDetails()
    {
        // Act
        var request = new CreatePaymentRequest
        {
            OrderNumber = "",
            Amount = 100m,
            Currency = "EUR",
            Description = null,
            GatewayId = "Montonio",
            UserId = "TEST_USER_ID"
        };

        using var response = await fixture.ApiClient.PostAsJsonAsync(
            "/api/payments",
            request,
            fixture.CancellationToken);


        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var responseBody = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(responseBody);

        var orderNumberErrors = responseBody.Errors["OrderNumber"];
        Assert.NotNull(orderNumberErrors);

        Assert.Equal("The OrderNumber field is required.", orderNumberErrors[0]);
    }

    [Fact]
    public async Task CreatePaymentEndpoint_WhenGatewayIdNotInEnumValues_Returns400WithCustomValidationError()
    {
        // Act
        var request = new CreatePaymentRequest
        {
            OrderNumber = "ORD_123454",
            Amount = 100m,
            Currency = "EUR",
            Description = null,
            GatewayId = "Monntonio",
            UserId = "TEST_USER_ID"
        };

        using var response = await fixture.ApiClient.PostAsJsonAsync(
            "/api/payments",
            request,
            fixture.CancellationToken);


        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var responseBody = await response.Content.ReadFromJsonAsync<ApiResponse>();
        Assert.NotNull(responseBody);

        Assert.Equal("Invalid payment gateway type: Monntonio", responseBody.Message);
    }
}
