namespace TicketAiTeam.Configuration.Extensions;

using System.ClientModel;
using System.Threading.Channels;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using OpenAI.Chat;

using TicketAiTeam.Services;
using TicketAiTeam.Web.Model;

using YouTrackSharp;

public static class ServiceCollectionExtensions
{
  public static void AddTaitMcpServer(this IServiceCollection services)
  {
    services.AddMcpServer().WithHttpTransport().WithToolsFromAssembly();
  }

  public static void AddYouTrackClient(this IServiceCollection services)
  {
    services.AddSingleton<Connection>(provider =>
    {
      var options = provider.GetRequiredService<IOptions<ApplicationOptions>>().Value;
      return new BearerTokenConnection(options.YouTrackUrl, options.YouTrackToken);
    });
  }

  public static void AddWebhookHandler(this IServiceCollection services)
  {
    services.AddHostedService<WebhookHandlerBackgroundService>();
    services.AddSingleton(_ => Channel.CreateUnbounded<WebhookEvent>(new() { SingleReader = true }));
    services.AddSingleton(provider => provider.GetRequiredService<Channel<WebhookEvent>>().Reader);
    services.AddSingleton(provider => provider.GetRequiredService<Channel<WebhookEvent>>().Writer);
  }

  public static void AddChatClient(this IServiceCollection services)
  {
    services.AddSingleton<ChatClient>(provider =>
    {
      var options = provider.GetRequiredService<IOptions<ApplicationOptions>>().Value;
      return new(options.ChatModel, new ApiKeyCredential(options.ChatToken), new() { Endpoint = options.ChatEndpoint });
    });
  }
}