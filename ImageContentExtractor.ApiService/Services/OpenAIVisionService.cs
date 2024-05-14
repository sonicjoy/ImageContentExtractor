using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;
using PDFtoImage;
using SkiaSharp;

namespace ImageContentExtractor.ApiService.Services;

public class OpenAIVisionService
{
    private readonly HttpClient _httpClient;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
	};

    

    private const string Prompt = "From the user manual, please locate the troubleshooting section and return the " +
                                  "section as an deserialisable json object in the following format:\r\n" +
                                  "{\r\n" +
                                  "  'problems': [\r\n" +
                                  "      {\r\n" +
                                  "        'problem': 'string',\r\n" +
                                  "        'page': int" +
                                  "        'causes': [\r\n" +
                                  "          {\r\n" +
                                  "            'cause': 'string',\r\n" +
                                  "            'solution': 'string'\r\n" +
                                  "          }\r\n" +
                                  "        ]\r\n" +
                                  "      }\r\n" +
                                  "    ]\r\n" +
                                  "}\r\n" +
                                  "IMPORTANT: do not add any elaboration as the response will be deserialised straight upon reception; \r\n" +
                                  "           include the page numbers of the problems in the json object";

    private const int MaxTokens = 2000;

    public OpenAIVisionService(string apiKey, string? orgId = null)
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

        if (!string.IsNullOrWhiteSpace(orgId))
        {
            _httpClient.DefaultRequestHeaders.Add("OpenAI-Organization", orgId);
        }

    }

    public async Task<string> ExtractTextFromImageAsync(string[] base64Strings)
    {
        var content = new List<OpenAIContent>
        {
	        new("text")
	        {
		        Text = Prompt
	        }
        };

        content.AddRange(base64Strings.Select(s => new OpenAIContent("image_url")
        {
            ImageUrl = new OpenAIImageUrl($"data:image/jpeg;base64,{s}")
        }));

        var requestBody = new OpenAIRequestBody("gpt-4o", new ResponseFormat("json_object"),
        [
	        new OpenAIMessage("user", content.ToArray())
        ], MaxTokens);

        var jsonData = JsonSerializer.Serialize(requestBody, _jsonSerializerOptions);

        var httpContent = new StringContent(jsonData, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", httpContent);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to analyze image: {response.StatusCode}");
        }

        try
        {
	        var responseContent = await response.Content.ReadFromJsonAsync<OpenAIResponse>(_jsonSerializerOptions);

	        return responseContent.Choices[0].Message.Content;
        }
        catch(JsonException ex)
		{
			Console.WriteLine(ex);
			throw;
		}

    }

    public static async Task<string[]> GetBase64StringFromPdfFile(IFormFile file)
	{
		try
		{
            await using MemoryStream pdfStream = new();

			await file.CopyToAsync(pdfStream);

            var pageImages = Conversion.ToImages(pdfStream)
             .Select(pageImage => pageImage.Encode(SKEncodedImageFormat.Jpeg, 100).ToArray())
             .Select(Convert.ToBase64String)
             .ToArray();

            return pageImages;
        }
		catch (Exception ex)
		{
            Console.WriteLine(ex);

            throw;
		}
	}

    #region OpenAI Models
    public readonly record struct OpenAIRequestBody(string Model, ResponseFormat ResponseFormat,  OpenAIMessage[] Messages, int MaxTokens);

    public readonly record struct OpenAIMessage(string Role, OpenAIContent[] Content);

    public readonly record struct OpenAIImageUrl(string Url);

    public record struct OpenAIContent(string Type)
    {
        public string? Text { get; set; }

        public OpenAIImageUrl? ImageUrl { get; set; }
    }

    public readonly record struct ResponseFormat(string Type);

    public readonly record struct OpenAIResponse(
	    string? Id,
	    string? Object,
	    int Created,
	    string? Model,
	    UsageInfo? Usage,
	    Choice[] Choices,
	    string? SystemFingerprint);

    public readonly record struct UsageInfo(
	    int PromptTokens,
	    int CompletionTokens,
	    int TotalTokens
	);

	public readonly record struct Message(string Role, string Content);

	public readonly record struct Choice(
		Message Message, 
		string? FinishReason, 
		int Index);

	#endregion
}