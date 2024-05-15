# Image Content Extractor

## Description
ImageContentExtractor is an Aspire.Net solution written in C# .NET that allows users to upload a manual in PDF format and extract the "troubleshooting" section in JSON format. 
The file upload endpoint will use OpenAI Vision Api to interpret the manual files.
The response as JSON string will be sent back asynchronously using SignalR channel.

## Prerequisites
Before running the solution, make sure you have set your OpenAI API key and organization ID in the secret file or appsettings.json or environment variables as follows:

```json
{
  "OpenAIServiceOptions:ApiKey": "sk-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
  "OpenAIServiceOptions:OrganizationId": "org-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
}
```

## How to Run
Please refer to the official page for how to setup and run the solution: [setup and run Aspire solution](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling)

Once the dashboard opens in a browser, click on the endpoint of the `webfrontend` project to open the home page where you can upload a PDF file and extract the "troubleshooting" section in JSON format.

The following urls are accessible when you run the solution locally.

For the web frontend that use the upload endpoint and receive websocket update
[https://localhost:7162](https://localhost:7162)

The upload file endpoint
[https://localhost:7487/upload-file](https://localhost:7487/upload-file)

Use ```/messagehub``` endpoint to connect to the SignalR Hub. Follow the this link to setup SignalR with TypeScript
[Configure SignalR in TypeScript](https://learn.microsoft.com/en-us/aspnet/core/tutorials/signalr-typescript-webpack?view=aspnetcore-8.0&tabs=visual-studio-code)




