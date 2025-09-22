namespace TicketAiTeam.Mcp.Model.YouTrack;

using System.ComponentModel;

using JetBrains.Annotations;

using YouTrackSharp.Issues;

[PublicAPI]
[Description("A full issue with every field.")]
public sealed class YouTrackIssue : YouTrackIssueSummary
{
  public new static YouTrackIssue FromIssue(Issue issue)
  {
    return new()
    {
      Id = issue.Id,
      Summary = issue.Summary,
      Description = issue.Description,
      State = issue.GetField(YouTrackFields.StateFieldName)?.AsString() ?? YouTrackFields.StateFallbackValue,
      Assignee = issue.GetField(YouTrackFields.AssigneeFieldName)?.AsString() ?? YouTrackFields.AssigneeFallbackValue
    };
  }
}