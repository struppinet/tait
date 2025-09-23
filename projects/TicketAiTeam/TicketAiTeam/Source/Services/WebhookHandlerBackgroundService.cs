namespace TicketAiTeam.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using OpenAI.Chat;

using TicketAiTeam.Mcp.Tools.YouTrack;
using TicketAiTeam.Web.Model;

using YouTrackSharp;

public sealed class WebhookHandlerBackgroundService : BackgroundService
{
  private readonly ILogger<WebhookHandlerBackgroundService> _logger;
  private readonly ChannelReader<WebhookEvent> _channel;
  private readonly ChatClient _chatClient;
  private readonly Connection _connection;

  public WebhookHandlerBackgroundService
  (
    ILogger<WebhookHandlerBackgroundService> logger,
    ChannelReader<WebhookEvent> channel,
    ChatClient chatClient,
    Connection connection
  )
  {
    _logger = logger;
    _channel = channel;
    _chatClient = chatClient;
    _connection = connection;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (!stoppingToken.IsCancellationRequested)
    {
      await Task.Delay(1000, stoppingToken);

      try
      {
        var @event = await _channel.ReadAsync(stoppingToken);
        await HandleEvent(@event);
      }
      catch (ChannelClosedException)
      {
        break;
      }
    }
  }

  private async Task HandleEvent(WebhookEvent @event)
  {
    _logger.LogInformation("{IssueId} state: {State} ({OldState})", @event.IssueId, @event.State, @event.OldState);

    var tool = ChatTool.CreateFunctionTool(
      "GetIssue",
      "Gets details about an issue from YouTrack.",
      BinaryData.FromString("""
                            {
                                "type": "object",
                                "properties": {
                                    "issueId": {
                                        "type": "string",
                                        "description": "The ID of the issue to get details for."
                                    }
                                },
                                "required": [ "issueId" ]
                            }
                            """));

    var completionOptions = new ChatCompletionOptions { Tools = { tool } };

    var messages = new List<ChatMessage>();
    messages.Add(ChatMessage.CreateSystemMessage("You are a helpful assistant. You can ask questions about issues in YouTrack."));
    messages.Add(ChatMessage.CreateUserMessage($"What's the status of issue {@event.IssueId}?"));

    bool requiresAction = true;
    while (requiresAction)
    {
      var completion = await _chatClient.CompleteChatAsync(messages, completionOptions);
      messages.Add(new AssistantChatMessage(completion));

      switch (completion.Value.FinishReason)
      {
        case ChatFinishReason.Stop:
          requiresAction = false;
          break;

        case ChatFinishReason.ToolCalls:
          requiresAction = true;
          foreach (var toolCall in completion.Value.ToolCalls)
          {
            switch (toolCall.FunctionName)
            {
              case "GetIssue":
              {
                using var arguments = JsonDocument.Parse(toolCall.FunctionArguments);
                if (!arguments.RootElement.TryGetProperty("issueId", out var issueIdElement))
                {
                  throw new ArgumentException("Missing issueId");
                }

                string issueId = issueIdElement.GetString() ?? throw new ArgumentException("Missing issueId");

                var issue = await IssueTools.GetIssue(issueId, _connection);

                messages.Add(new ToolChatMessage(toolCall.Id, JsonSerializer.Serialize(issue)));
                break;
              }

              default:
                throw new ArgumentOutOfRangeException();
            }
          }

          break;

        default:
          throw new NotImplementedException();
      }

      if (!requiresAction)
      {
        _logger.LogInformation("Got response: {Response}", completion.Value.Content.FirstOrDefault()?.Text ?? "<empty>");
      }
    }
  }
}