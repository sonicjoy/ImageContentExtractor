var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.ImageContentExtractor_ApiService>("apiservice")
	.WithHttpsEndpoint(port: 4444, name: "messagehub");

builder.AddProject<Projects.ImageContentExtractor_Web>("webfrontend")
	.WithExternalHttpEndpoints()
	.WithReference(apiService);

builder.Build().Run();
