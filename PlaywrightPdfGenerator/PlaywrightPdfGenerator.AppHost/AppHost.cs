var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.PlaywrightPdfGenerator_ApiService>("api")
    .WithHttpHealthCheck("/health");

builder.Build().Run();
