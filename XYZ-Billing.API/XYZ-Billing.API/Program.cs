using Aspire.StackExchange.Redis;
using Carter;
using Serilog;
using System.Text.Json.Serialization;
using WireMock.Server;
using XYZ.Billing.API;
using XYZ.Billing.API.Cache;
using XYZ.Billing.API.Middlewares;
using XYZ.Billing.API.PaymentGateways;
using XYZ.Billing.API.PaymentGateways.Montonio;
using XYZ.Billing.API.PaymentGateways.Stripe;
using XYZ.Billing.API.Persistence;

var mockServer = WireMockServer.Start(port: 9876);
mockServer.AddMockGateways();

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

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

// only for local development, in production we would use a real redis instance
builder.AddRedisClient("redis");

builder.Services.AddCacheServices();
builder.Services.AddPaymentGateways();
builder.Services.AddDatabase();

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddCarter();

builder.Services.AddCors(options =>
{
    options.AddPolicy("XYZ-App", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.MapCarter();

app.UseCors("XYZ-App");

app.MapDefaultEndpoints();

app.Run();
