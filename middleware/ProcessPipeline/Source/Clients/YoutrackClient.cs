using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessPipeline.Clients
{
    public class YoutrackClient
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json;
        
        public const string BaseUrl = "http://localhost:8080";

        // baseUrl example: https://youtrack.example.com
        // token: Permanent token with "YouTrack" scope, value like "perm:XXXXX..."
        public YoutrackClient(HttpClient httpClient, string token)
        {
            _http = httpClient;
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));

            _json = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
        }
        
        // ... existing code ...
        // Adds a comment to the specified issue (issueIdReadable like "PRJ-123")
        public async Task AddCommentAsync(string issueIdReadable, string text, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(issueIdReadable)) throw new ArgumentException("Issue id must be provided", nameof(issueIdReadable));
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentException("Text must be provided", nameof(text));

            var payload = new
            {
                text
            };

            using var content = new StringContent(JsonSerializer.Serialize(payload, _json), Encoding.UTF8, MediaTypeNames.Application.Json);

            // YouTrack REST API endpoint for creating an issue comment:
            // POST /api/issues/{issueId}/comments
            var uri = $"/api/issues/{Uri.EscapeDataString(issueIdReadable)}/comments?fields=id,text,created,author(login,name)";

            using var resp = await _http.PostAsync(uri, content, ct).ConfigureAwait(false);
            if (!resp.IsSuccessStatusCode)
            {
                var body = await SafeReadAsync(resp, ct);
                throw new HttpRequestException($"YouTrack comment failed ({(int)resp.StatusCode} {resp.ReasonPhrase}). Body: {body}");
            }
        }

        private static async Task<string> SafeReadAsync(HttpResponseMessage resp, CancellationToken ct)
        {
            try
            {
                return await resp.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}