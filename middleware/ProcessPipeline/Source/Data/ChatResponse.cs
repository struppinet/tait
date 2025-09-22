using System.Collections.Generic;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace ProcessPipeline.Data;

[PublicAPI]
public sealed class ChatResponse
{
    public string Id { get; set; } = "";
    public string Object { get; set; } = "";
    public long Created { get; set; }
    public string Model { get; set; } = "";
    public List<Choice> Choices { get; set; } = new();
    public Usage Usage { get; set; } = new();
    public Dictionary<string, object>? Stats { get; set; }
    [JsonPropertyName("system_fingerprint")]
    public string? SystemFingerprint { get; set; }
}

[PublicAPI]
public sealed class Choice
{
    public int Index { get; set; }
    public ChatMessage Message { get; set; } = new();
    public object? Logprobs { get; set; }
    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; set; }
}

[PublicAPI]
public sealed class ChatMessage
{
    public string Role { get; set; } = "";
    public string Content { get; set; } = "";
    [JsonPropertyName("tool_calls")]
    public List<object> ToolCalls { get; set; } = new();
}

[PublicAPI]
public sealed class Usage
{
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; set; }

    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; set; }

    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }
}