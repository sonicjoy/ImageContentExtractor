# Image Content Extractor

## Description
ImageContentExtractor is an Aspire.Net solution written in C# .NET that allows users to upload a manual in PDF format and extract the "troubleshooting" section in JSON format. 

## Prerequisites
Before running the solution, make sure you have set your OpenAI API key and organization ID in the secret file as follows:

```
{
  "OpenAIServiceOptions:ApiKey": "sk-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
  "OpenAIServiceOptions:OrganizationId": "org-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
}
```
## How to Run
Please refer to the official page: [setup and run Aspire solution](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling?tabs=dotnet-cli%2Cwindows#net-aspire-dashboard)

To run the solution, use the following dotnet CLI command:

`.NET CLI`
``` 
dotnet run --project ImageContentExtractor.AppHost
```

Once the dashboard opens in a browser, click on the endpoint of the `webfrontend` project to open the home page where you can upload a PDF file and extract the "troubleshooting" section in JSON format.
