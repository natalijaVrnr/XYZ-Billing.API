using Carter;
using Microsoft.AspNetCore.Hosting.Server;
using Serilog;
using WireMock.Server;
using XYZ.Billing.API;
using XYZ.Billing.API.Middlewares;
using XYZ.Billing.API.PaymentGateways.Montonio;
using XYZ.Billing.API.PaymentGateways.Stripe;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddValidation();

builder.Services.AddHttpClient<StripePaymentGateway>(client =>
{
    client.BaseAddress = new Uri("http://localhost:9876/stripe");
});

builder.Services.AddHttpClient<MontonioPaymentGateway>(client =>
{
    client.BaseAddress = new Uri("http://localhost:9876/montonio");
});

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddCarter();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.MapCarter();

app.Run();

var mockServer = WireMockServer.Start(port: 9876);
mockServer.AddMockGateways();
