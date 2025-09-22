using System;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ProcessPipeline.Clients;
using ProcessPipeline.Data;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<LmStudioClient>(client =>
{
    client.BaseAddress = new Uri(LmStudioClient.BaseUrl);
});
builder.Services.AddHttpClient<YoutrackClient>(client =>
{
    client.BaseAddress = new Uri(YoutrackClient.BaseUrl);
})
.AddTypedClient((http, sp) => new YoutrackClient(http, "perm-YWRtaW4=.NDMtMA==.Mg0yXJkcLWcSGX0nXvEcTm2zoczScx"));

var app = builder.Build();

app.MapPost("/youtrack/webhook", async (HttpRequest req, LmStudioClient lm, YoutrackClient yt) =>
{
    Console.WriteLine($"Method: {req.Method}, Path: {req.Path}");
    // console log the json body
    req.EnableBuffering();
    using var reader = new System.IO.StreamReader(req.Body, leaveOpen: true);
    var body = await reader.ReadToEndAsync();
    req.Body.Position = 0;
    Console.WriteLine("Body:");
    Console.WriteLine(string.IsNullOrWhiteSpace(body) ? "<empty>" : body);

    // var secret = req.Headers["X-YouTrack-Signature"].ToString();
    // if (secret != "replace-with-strong-shared-secret")
    //     return Results.Unauthorized();

    // parse the json body to YoutrackEvent
    req.EnableBuffering();
    var youtrackEvent = await JsonSerializer.DeserializeAsync<YoutrackEvent>(
        req.Body,
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    req.Body.Position = 0;

    if (youtrackEvent is null)
        return Results.BadRequest("Invalid YouTrack payload.");
    
    // skip updates
    if (youtrackEvent.EventType == "Updated")
    {
        return Results.Ok();
    }
    
    // get the description of the event
    var requestMessage = youtrackEvent.Fields.FirstOrDefault(f => f.Title == "Description")?.Value ?? "";

    // Create a new ChatRequest
    var chatRequest = new ChatRequest
    {
        Model = "qwen/qwen3-4b-thinking-250",
        Messages = new ()
        {
            new Message
            {
                Role = "system",
                Content = "Use short answers and try to be precise."
            },
            new Message
            {
                Role = "user",
                Content = requestMessage ?? ""
            }
        }
    };

    // LmStudioClient call
    var completion = await lm.CreateChatCompletionAsync(chatRequest);
     
    // remove all text that is between the <think> and </think> strings
    var rawContent = completion.Choices.First().Message.Content ?? string.Empty;
    var startTag = "<think>";
    var endTag = "</think>";
    while (true)
    {
        var start = rawContent.IndexOf(startTag, StringComparison.OrdinalIgnoreCase);
        if (start < 0) break;
        var end = rawContent.IndexOf(endTag, start + startTag.Length, StringComparison.OrdinalIgnoreCase);
        if (end < 0) { rawContent = rawContent.Remove(start, startTag.Length); break; }
        rawContent = rawContent.Remove(start, (end + endTag.Length) - start);
    }
    var cleanedContent = rawContent.Trim();
    
    
    // Respond in Youtrack
    await yt.AddCommentAsync(youtrackEvent.IssueId, cleanedContent);
    
    Console.WriteLine("LM Studio response: ");
    Console.WriteLine(cleanedContent);

    // Return Ok
    return Results.Ok();
});

app.MapFallback(async (HttpContext ctx) =>
{
    Console.WriteLine($"Method: {ctx.Request.Method}, Path: {ctx.Request.Path}");
    foreach (var header in ctx.Request.Headers)
    {
        Console.WriteLine($"Header: {header.Key} = {header.Value}");
    }

    ctx.Request.EnableBuffering();
    using var reader = new System.IO.StreamReader(ctx.Request.Body, leaveOpen: true);
    var body = await reader.ReadToEndAsync();
    ctx.Request.Body.Position = 0;

    Console.WriteLine("Body:");
    Console.WriteLine(string.IsNullOrWhiteSpace(body) ? "<empty>" : body);

    ctx.Response.StatusCode = StatusCodes.Status404NotFound;
    await ctx.Response.WriteAsync("Not Found");
});

await app.RunAsync();