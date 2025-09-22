namespace TicketAiTeam.Mcp.Tools.YouTrack;

using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using JetBrains.Annotations;

using ModelContextProtocol.Server;

using YouTrackSharp;
using YouTrackSharp.Projects;

[PublicAPI]
[McpServerToolType]
public class ProjectTools
{
  [McpServerTool(Name = nameof(GetProjects))]
  [Description("Get all projects from YouTrack")]
  [return: Description("A JSON array of projects")]
  public static async Task<Project[]> GetProjects
  (
    Connection connection
  )
  {
    var service = connection.CreateProjectsService();
    var projects = await service.GetAccessibleProjects(true);

    return projects.ToArray();
  }
}