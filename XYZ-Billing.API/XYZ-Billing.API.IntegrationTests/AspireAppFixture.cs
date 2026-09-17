using Aspire.Hosting;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace XYZ.Billing.API.IntegrationTests;

[CollectionDefinition("AspireApp")]
public class AspireAppCollection : ICollectionFixture<AspireAppFixture> { }

public class AspireAppFixture : IAsyncLifetime
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60);
    private IDistributedApplicationTestingBuilder? _appHost;
    private DistributedApplication _app;
    public HttpClient ApiClient { get; private set; }
    public CancellationToken CancellationToken { get; private set; }
    public async Task InitializeAsync()
    {
        var cts = new CancellationTokenSource(delay: DefaultTimeout);
        CancellationToken = cts.Token;

        _appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.XYZ_Billing_API_AppHost>(CancellationToken);
        _appHost.Services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Debug);
            logging.AddFilter(_appHost.Environment.ApplicationName, LogLevel.Debug);
            logging.AddFilter("Aspire.", LogLevel.Debug);
        });
        _appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        _app = await _appHost.BuildAsync(CancellationToken).WaitAsync(DefaultTimeout, CancellationToken);
        await _app.StartAsync(CancellationToken).WaitAsync(DefaultTimeout, CancellationToken);

        ApiClient = _app.CreateHttpClient("api");

        await _app.ResourceNotifications.WaitForResourceAsync("api", "Running", CancellationToken)
            .WaitAsync(DefaultTimeout, CancellationToken);
    }

    public async Task DisposeAsync()
    {
        await _app.DisposeAsync();
    }
}