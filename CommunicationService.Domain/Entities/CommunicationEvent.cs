namespace CommunicationService.Domain.Entities;

public class CommunicationEvent
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenantId { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty; // appointment.created, invoice.paid
    public string SourceService { get; set; } = string.Empty; // scheduling-service, finance-service
    public string SourceEntityId { get; set; } = string.Empty;
    public string Payload { get; set; } = "{}"; // JSON
    public string Status { get; set; } = CommunicationEventStatus.Received;
    public DateTime? ProcessedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public static class CommunicationEventStatus
{
    public const string Received = "received";
    public const string Processed = "processed";
    public const string Failed = "failed";
}
