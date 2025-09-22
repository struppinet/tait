namespace TicketAiTeam.Services;

using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using OpenAI.Chat;

using TicketAiTeam.Web.Model;

public sealed class WebhookHandlerBackgroundService : BackgroundService
{
  private readonly ILogger<WebhookHandlerBackgroundService> _logger;
  private readonly ChannelReader<WebhookEvent> _channel;
  private readonly ChatClient _chatClient;

  public WebhookHandlerBackgroundService
  (
    ILogger<WebhookHandlerBackgroundService> logger,
    ChannelReader<WebhookEvent> channel,
    ChatClient chatClient
  )
  {
    _logger = logger;
    _channel = channel;
    _chatClient = chatClient;
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
    _logger.LogInformation("Got {EventType} for {IssueId}", @event.EventType, @event.IssueId);

    var message = ChatMessage.CreateUserMessage($"What's the status of issue {@event.IssueId}?");

    var result = await _chatClient.CompleteChatAsync(message);

    _logger.LogInformation("Got completion: {Completion}", result.Value.Content.FirstOrDefault()?.Text ?? "");
  }
}