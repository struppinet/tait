using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ProcessPipeline.Data;

namespace ProcessPipeline.Clients;

public class LmStudioClient
{
    private readonly HttpClient _http;

    public const string BaseUrl = "http://localhost:1234";
    public const string ChatCompletionsEndpoint = "/v1/chat/completions";

    public LmStudioClient(HttpClient httpClient)
    {
        _http = httpClient;
    }
    
    public async Task<ChatResponse> CreateChatCompletionAsync(ChatRequest payload, CancellationToken cancellationToken = default)
    {
        using var response = await _http.PostAsJsonAsync(ChatCompletionsEndpoint, payload, cancellationToken);
        response.EnsureSuccessStatusCode();
        // Deserialize JSON into ChatResponse with case-insensitive property matching
        var chatResponse = await response.Content.ReadFromJsonAsync<ChatResponse>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            cancellationToken
        );
        return chatResponse ?? new ChatResponse();
    }

}