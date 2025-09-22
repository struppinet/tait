namespace TicketAiTeam.Mcp.Model.YouTrack;

using System.ComponentModel;

using JetBrains.Annotations;

using YouTrackSharp.Issues;

[PublicAPI]
[Description("A summary of an issue with summary, description and state.")]
public class YouTrackIssueSummary
{
  [Description("The ID of the issue.")]
  public required string Id { get; set; }

  [Description("The summary of the issue.")]
  public required string Summary { get; set; }

  [Description("The description of the issue.")]
  public required string Description { get; set; }

  [Description("The state of the issue.")]
  public required string State { get; set; }

  [Description("The person the issue is assigned to.")]
  public string? Assignee { get; set; }

  public static YouTrackIssueSummary FromIssue(Issue issue)
  {
    return new()
    {
      Id = issue.Id,
      Summary = issue.Summary,
      Description = issue.Description,
      State = issue.GetField(YouTrackFields.StateFieldName)?.AsString() ?? YouTrackFields.StateFallbackValue,
      Assignee = issue.GetField(YouTrackFields.AssigneeFieldName)?.AsString() ?? YouTrackFields.AssigneeFallbackValue,
    };
  }
}