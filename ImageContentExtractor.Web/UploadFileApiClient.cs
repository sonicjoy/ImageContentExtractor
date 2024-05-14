using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Forms;

namespace ImageContentExtractor.Web;

public class UploadFileApiClient(HttpClient httpClient)
{
	private const long MaxFileSize = 1024 * 1024 * 20;

	public async Task<HttpResponseMessage> UploadFile(IBrowserFile file, CancellationToken cancellationToken = default)
    {
	    using var content = new MultipartFormDataContent();

	    var fileContent = new StreamContent(file.OpenReadStream(MaxFileSize));

	    fileContent.Headers.ContentType = 
		    new MediaTypeHeaderValue(file.ContentType);

	    content.Add(
		    content: fileContent,
		    name: "file",
		    fileName: file.Name);

	    var response = await httpClient.PostAsync("/upload-file", content, cancellationToken);

	    // Check if the request was successful
	    if (response.IsSuccessStatusCode)
	    {
		    return response;
	    }

	    // If the request failed, throw an exception or handle the error
	    throw new Exception($"Failed to upload file: {response.StatusCode}");


    }
}

public readonly record struct Troubleshooting(ProblemType[] Problems);

public readonly record struct ProblemType(string Problem, CauseType[] Causes, int Page);

public readonly record struct CauseType(string Cause, string Solution);
