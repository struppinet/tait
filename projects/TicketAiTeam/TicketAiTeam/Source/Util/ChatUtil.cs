namespace TicketAiTeam.Util;

using System;

public static class ChatUtil
{
  public static string ClearThinking(string rawContent)
  {
    const string startTag = "<think>";
    const string endTag = "</think>";
    
    while (true)
    {
      int start = rawContent.IndexOf(startTag, StringComparison.OrdinalIgnoreCase);
      if (start < 0) break;
      int end = rawContent.IndexOf(endTag, start + startTag.Length, StringComparison.OrdinalIgnoreCase);
      if (end < 0) { rawContent = rawContent.Remove(start, startTag.Length); break; }
      rawContent = rawContent.Remove(start, (end + endTag.Length) - start);
    }
    return rawContent.Trim();
  }
}