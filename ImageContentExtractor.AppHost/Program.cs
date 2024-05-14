var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.ImageContentExtractor_ApiService>("apiservice");

builder.AddProject<Projects.ImageContentExtractor_Web>("webfrontend")
	.WithExternalHttpEndpoints()
	.WithReference(apiService);

builder.Build().Run();
