namespace CommunicationService.Domain.Entities;

public class ScheduledMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenantId { get; set; } = string.Empty;
    public string? EventId { get; set; }
    public string SourceEntityId { get; set; } = string.Empty;
    public string MessageType { get; set; } = string.Empty; // reminder_24h, reminder_1h, follow_up
    public DateTime ScheduledAt { get; set; }
    public string Status { get; set; } = ScheduledMessageStatus.Pending;
    public DateTime? SentAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public static class ScheduledMessageStatus
{
    public const string Pending = "pending";
    public const string Sent = "sent";
    public const string Cancelled = "cancelled";
}
