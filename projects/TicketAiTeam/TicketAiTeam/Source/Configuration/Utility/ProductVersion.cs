namespace TicketAiTeam.Configuration.Utility;

using System.Reflection;

/// <summary>
///   Gets the current version of the application.
/// </summary>
public static class ProductVersion
{
  /// <summary>
  ///   The current version of the application as provided by <see cref="AssemblyInformationalVersionAttribute"/>.
  /// </summary>
  public static readonly string Current = Assembly.GetEntryAssembly()?
    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
    .InformationalVersion ?? "0.0.0-unknown";
}