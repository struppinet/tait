namespace TicketAiTeam.Configuration.Extensions;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

using TicketAiTeam.Configuration.Utility;

/// <summary>
///   
/// </summary>
public static class WebApplicationExtensions
{
  /// <summary>
  ///   Writes several informational values about the runtime environment to the log when the application starts.
  /// </summary>
  /// <param name="app">The <see cref="WebApplication"/> to configure.</param>
  public static void LogEnvironmentInformationOnStartup(this WebApplication app)
  {
    app.Lifetime.ApplicationStarted.Register(() =>
    {
      app.Logger.LogInformation("Product: {ProductName}", ProductName.Current);
      app.Logger.LogInformation("Version: {ProductVersion}", ProductVersion.Current);
      app.Logger.LogInformation("Environment: {EnvironmentName}", app.Environment.EnvironmentName);
      app.Logger.LogInformation("Listening on: {Endpoints}", string.Join(", ", app.Urls));
    });
  }
}