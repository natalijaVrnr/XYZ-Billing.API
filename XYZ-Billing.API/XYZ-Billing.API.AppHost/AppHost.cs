var builder = DistributedApplication.CreateBuilder(args);

var redis = builder.AddRedis("redis");

var api = builder.AddProject<Projects.XYZ_Billing_API>("api")
    .WithHttpEndpoint(port: 7229, name: "http")
    .WithHttpsEndpoint(port: 5202, name: "https")
    .WithHttpHealthCheck()
    .WithReference(redis);

builder.Build().Run();
