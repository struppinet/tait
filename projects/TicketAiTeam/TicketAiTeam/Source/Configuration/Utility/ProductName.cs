namespace TicketAiTeam.Configuration.Utility;

using System.Reflection;

/// <summary>
///   Gets the name of the current application.
/// </summary>
public static class ProductName
{
  /// <summary>
  ///   The name of the current application based on the entry assembly.
  /// </summary>
  public static readonly string Current = Assembly.GetEntryAssembly()?.GetName().Name ?? "Unknown";
}