using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;
using XYZ.Billing.API.Cache;
using XYZ.Billing.API.Domain.Enums;
using XYZ.Billing.API.Domain.Models;
using XYZ.Billing.API.PaymentGateways.Dtos;
using XYZ.Billing.API.Persistence;

namespace XYZ.Billing.API.PaymentGateways.Tests;

public class PaymentServiceTests
{
    private readonly DatabaseContext _db;
    private readonly IPaymentIdempotencyGuard _guard;
    private readonly IPaymentGateway _montonioGateway;

    private readonly IPaymentGateway _stripeGateway;
    private readonly PaymentService _sut;

    public PaymentServiceTests()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new DatabaseContext(options);

        _guard = Substitute.For<IPaymentIdempotencyGuard>();

        _montonioGateway = Substitute.For<IPaymentGateway>();
        _stripeGateway = Substitute.For<IPaymentGateway>();

        var services = new ServiceCollection();

        services.AddKeyedSingleton(
            PaymentGatewayType.Montonio,
            _montonioGateway);

        services.AddKeyedSingleton(
            PaymentGatewayType.Stripe,
            _stripeGateway);

        var serviceProvider = services.BuildServiceProvider();

        _sut = new PaymentService(
            serviceProvider,
            _guard,
            _db);
    }

    [Fact]
    public async Task ProcessPaymentAsync_WhenPaymentDoesNotExistNeitherInCacheNorInDb_CallsPaymentGatewayAndCreatesPayment()
    {
        // Arrange
        var orderNumber = "ORD_123454";

        var paymentDto = new GatewayPaymentCreationRequest(
            OrderNumber: orderNumber,
            Amount: 100m,
            Currency: "EUR",
            Description: null,
            GatewayId: PaymentGatewayType.Montonio,
            UserId: "TEST_USER_ID");

        // configure cache to return true, aka we succeeded to claim the process
        _guard.TryStartPaymentAsync(Arg.Any<string>(), Arg.Any<TimeSpan>()).Returns(true);
        // and we succeeded to release lock
        _guard.ReleasePaymentAsync(Arg.Any<string>()).Returns(true);

        var paymentId = Guid.Parse("5a63218d-3486-4fb0-88b2-c766e903641d");

        var paymentConfirmation = new PaymentConfirmationResponse(DateTime.Now, paymentId);

        _montonioGateway.ProcessPaymentAsync(Arg.Any<GatewayPaymentCreationRequest>())
            .Returns(paymentConfirmation);

        // Act
        var result = await _sut.ProcessPaymentAsync(paymentDto);

        // Assert
        await _montonioGateway.Received(1)
            .ProcessPaymentAsync(paymentDto);

        await _guard.Received(1)
            .ReleasePaymentAsync(orderNumber);

        Assert.Equal(PaymentStatus.Succeeded, result.Status);

        var payment = await _db.Payments
            .SingleAsync(x => x.OrderNumber == orderNumber);

        Assert.NotNull(payment);
        Assert.Equal(orderNumber, payment.OrderNumber); 
        Assert.Equal(paymentId, payment.Id);
        Assert.Equal(payment.Id, result.Confirmation?.PaymentId);
    }

    [Fact]
    public async Task ProcessPaymentAsync_WhenPaymentExistsInCache_ReturnsPaymentStatusInProgress()
    {
        // Arrange
        var orderNumber = "ORD_123454";

        var paymentDto = new GatewayPaymentCreationRequest(
            OrderNumber: orderNumber,
            Amount: 100m,
            Currency: "EUR",
            Description: null,
            GatewayId: PaymentGatewayType.Montonio,
            UserId: "TEST_USER_ID");

        // configure cache to return false, aka someone else claimed the process
        _guard.TryStartPaymentAsync(Arg.Any<string>(), Arg.Any<TimeSpan>()).Returns(false);

        // Act
        var result = await _sut.ProcessPaymentAsync(paymentDto);

        // Assert
        await _montonioGateway.Received(0)
            .ProcessPaymentAsync(paymentDto);

        await _guard.Received(0)
            .ReleasePaymentAsync(orderNumber);

        Assert.Equal(PaymentStatus.InProgress, result.Status);
    }

    [Fact]
    public async Task ProcessPaymentAsync_WhenPaymentNotInCacheButExistsInDb_ReturnsPaymentStatusSucceeded()
    {
        // Arrange
        var orderNumber = "ORD_123454";
        var paymentId = Guid.Parse("5a63218d-3486-4fb0-88b2-c766e903641d");

        _guard.TryStartPaymentAsync(Arg.Any<string>(), Arg.Any<TimeSpan>()).Returns(true);

        var payment = new Payment
        {
            Id = paymentId,
            OrderNumber = orderNumber,
            Amount = 100m,
            Currency = "EUR",
            Description = null,
            GatewayId = PaymentGatewayType.Montonio,
            UserId = "TEST_USER_ID"
        };

        _db.Payments.Add(payment);

        await _db.SaveChangesAsync();

        var paymentDto = new GatewayPaymentCreationRequest(
            OrderNumber: orderNumber,
            Amount: payment.Amount,
            Currency: payment.Currency,
            Description: payment.Description,
            GatewayId: payment.GatewayId,
            UserId: payment.UserId);

        // Act
        var result = await _sut.ProcessPaymentAsync(paymentDto);

        // Assert
        await _montonioGateway.Received(0)
            .ProcessPaymentAsync(paymentDto);

        Assert.Equal(PaymentStatus.Succeeded, result.Status);
    }
    [Fact]
    public async Task ProcessPaymentAsync_ResolvesGatewayServiceImplementationByGatewayId()
    {
        // Arrange
        var orderNumber = "ORD_123454";
        var paymentId = Guid.Parse("5a63218d-3486-4fb0-88b2-c766e903641d");

        var paymentDto = new GatewayPaymentCreationRequest(
            OrderNumber: orderNumber,
            Amount: 100m,
            Currency: "EUR",
            Description: null,
            GatewayId: PaymentGatewayType.Stripe,
            UserId: "TEST_USER_ID");

        _guard.TryStartPaymentAsync(Arg.Any<string>(), Arg.Any<TimeSpan>()).Returns(true);
        _guard.ReleasePaymentAsync(Arg.Any<string>()).Returns(true);

        var paymentConfirmation = new PaymentConfirmationResponse(DateTime.Now, paymentId);

        _stripeGateway.ProcessPaymentAsync(Arg.Any<GatewayPaymentCreationRequest>())
            .Returns(paymentConfirmation);

        // Act
        var result = await _sut.ProcessPaymentAsync(paymentDto);

        // Assert
        await _stripeGateway.Received(1)
            .ProcessPaymentAsync(paymentDto);

        await _montonioGateway.Received(0)
            .ProcessPaymentAsync(paymentDto);
    }
}
