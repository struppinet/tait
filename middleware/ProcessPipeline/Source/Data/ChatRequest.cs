using System.Collections.Generic;
using JetBrains.Annotations;

namespace ProcessPipeline.Data;

[PublicAPI]
public sealed class ChatRequest
{
    public string Model { get; set; } = "";

    // New properties to match the target JSON shape
    public List<Message> Messages { get; set; } = new();

    public double Temperature { get; set; } = 0.7;

    // Use int for -1 sentinel; consider switching to nullable int if needed
    public int Max_Tokens { get; set; } = -1;

    public bool Stream { get; set; } = false;
}

[PublicAPI]
public sealed class Message
{
    public string Role { get; set; } = "";
    public string Content { get; set; } = "";
}