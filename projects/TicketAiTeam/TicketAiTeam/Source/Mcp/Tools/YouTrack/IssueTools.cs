namespace TicketAiTeam.Mcp.Tools.YouTrack;

using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using JetBrains.Annotations;

using ModelContextProtocol.Server;

using TicketAiTeam.Mcp.Model.YouTrack;

using YouTrackSharp;
using YouTrackSharp.Issues;

[PublicAPI]
[McpServerToolType]
public static class IssueTools
{
  [McpServerTool(Name = nameof(FindIssues))]
  [Description("Finds issues within a single project from YouTrack")]
  [return: Description("A JSON array of issues with summary and description containing Markdown")]
  public static async Task<YouTrackIssueSummary[]> FindIssues
  (
    Connection connection,
    [Description("An optional query to filter the issues by.")]
    string? filter
  )
  {
    var service = connection.CreateIssuesService();
    var issues = await service.GetIssues(filter);

    return issues.Select(YouTrackIssueSummary.FromIssue).ToArray();
  }

  [McpServerTool(Name = nameof(GetIssue))]
  [Description("Get a single issue from YouTrack")]
  [return: Description("A JSON object of the issue with summary and description containing Markdown")]
  public static async Task<YouTrackIssue> GetIssue
  (
    [Description("The ID of the issue to get.")]
    string issueId,
    Connection connection
  )
  {
    var service = connection.CreateIssuesService();
    var issue = await service.GetIssue(issueId);

    return YouTrackIssue.FromIssue(issue);
  }

  [McpServerTool(Name = nameof(CreateIssue))]
  [Description("Create a new issue in YouTrack")]
  [return: Description("The ID of the created issue")]
  public static async Task<string> CreateIssue
  (
    Connection connection,
    [Description("The short name of the project to create the issue in.")]
    string projectShortName,
    [Description("The issue to create.")] YouTrackIssue issue
  )
  {
    var service = connection.CreateIssuesService();

    var request = new Issue();
    request.Summary = issue.Summary;
    request.Description = issue.Description;
    request.SetField(YouTrackFields.StateFieldName, issue.State);
    request.SetField(YouTrackFields.AssigneeFieldName, issue.Assignee);

    return await service.CreateIssue(projectShortName, request);
  }

  [McpServerTool(Name = nameof(TransitionIssue))]
  [Description("Transition an issue to a new state in YouTrack")]
  [return: Description("True if the transition was successful")]
  public static async Task<bool> TransitionIssue
  (
    Connection connection,
    [Description("The ID of the issue to transition.")]
    string issueId,
    [Description("The target state to transition to.")]
    string targetState
  )
  {
    var service = connection.CreateIssuesService();
    await service.ApplyCommand(issueId, $"State {targetState}", disableNotifications: true);

    return true;
  }

  public static async Task<bool> LinkIssues
  (
    Connection connection,
    string fromIssueId,
    string targetIssueId,
    string linkType
  )
  {
    var service = connection.CreateIssuesService();
    await service.ApplyCommand(fromIssueId, $"{linkType} {targetIssueId}", disableNotifications: true);

    return true;
  }
}