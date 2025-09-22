namespace TicketAiTeam.Web.Model;

using JetBrains.Annotations;

[PublicAPI]
public sealed class WebhookEvent
{
  public required string IssueId { get; set; }
  public required string EventType { get; set; }
}