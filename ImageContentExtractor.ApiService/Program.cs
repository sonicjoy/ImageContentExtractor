using ImageContentExtractor.ApiService.Hubs;
using ImageContentExtractor.ApiService.Services;
using ImageContentExtractor.ServiceDefaults;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddSignalR();

builder.Services.AddResponseCompression(opts =>
{
	opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
		new[] { "application/octet-stream" });
});

var app = builder.Build();

app.UseResponseCompression();
// Configure the HTTP request pipeline.
app.UseExceptionHandler();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

app.MapPost("/upload-file", async (IFormFile file) =>
{
	try
	{
		var base64Strings = await OpenAIVisionService.GetBase64StringFromPdfFile(file);

		var openAiApiKey = builder.Configuration["OpenAIServiceOptions:ApiKey"];

		var openAiOrgId = builder.Configuration["OpenAIServiceOptions:OrganizationId"];

		Task.Run(async () =>
		{
			try
			{
				var openAIService = new OpenAIVisionService(openAiApiKey, openAiOrgId);

				var response = await openAIService.ExtractTextFromImageAsync(base64Strings);

				var hub = app.Services.GetRequiredService<IHubContext<MessageHub>>();

				await hub.Clients.All.SendAsync("ReceiveMessage", response);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				throw;
			}
		});

	}
	catch(Exception ex)
	{
		Console.WriteLine(ex);
		throw;
	}


	return Results.Ok();

}).DisableAntiforgery();

app.MapDefaultEndpoints();

app.MapHub<MessageHub>("/messagehub");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
