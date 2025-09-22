namespace TicketAiTeam.Configuration;

using System;
using System.ComponentModel.DataAnnotations;

using JetBrains.Annotations;

[PublicAPI]
public class ApplicationOptions
{
  public const string SectionName = "Application";

  /// <summary>
  ///   The URL of the YouTrack instance.
  /// </summary>
  [Required]
  public required string YouTrackUrl { get; init; }

  /// <summary>
  ///   The API token used to authenticate with the YouTrack instance.
  /// </summary>
  [Required]
  public required string YouTrackToken { get; init; }

  /// <summary>
  ///   The URL of the ChatGPT instance.
  /// </summary>
  [Required]
  public required Uri ChatEndpoint { get; init; }

  /// <summary>
  ///   The name of the model to use in requests sent to the service.
  /// </summary>
  public string ChatModel { get; init; } = "openai/gpt-oss-120b";

  /// <summary>
  ///   The API token used to authenticate with the ChatGPT instance.
  /// </summary>
  [Required]
  public required string ChatToken { get; init; }
}