using JetBrains.Annotations;

namespace ProcessPipeline.Data;

[PublicAPI]
public sealed class YoutrackEvent
{
    public required string IssueId { get; set; }
    public required string EventType { get; set; }
    public FieldEntry[] Fields { get; set; } = [];
    public Attachment[] Attachments { get; set; } = [];

    [PublicAPI]
    public sealed class Attachment
    {
        public string? Fallback { get; set; }
        public string? Pretext { get; set; }
        public string? Color { get; set; }
    }

    [PublicAPI]
    public sealed class FieldEntry
    {
        public required string Title { get; set; }
        public required string Value { get; set; }
        public string? OldValue { get; set; }
        public bool? Changed { get; set; }
    }
}
