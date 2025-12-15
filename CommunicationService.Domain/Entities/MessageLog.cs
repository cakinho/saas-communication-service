namespace CommunicationService.Domain.Entities;

public class MessageLog
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenantId { get; set; } = string.Empty;
    public string? EventId { get; set; }
    public string Channel { get; set; } = string.Empty; // whatsapp, email, sms
    public string Recipient { get; set; } = string.Empty;
    public string TemplateKey { get; set; } = string.Empty;
    public string MessageBody { get; set; } = string.Empty;
    public string Status { get; set; } = MessageStatus.Pending;
    public int RetryCount { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public static class MessageStatus
{
    public const string Pending = "pending";
    public const string Sent = "sent";
    public const string Failed = "failed";
    public const string PermanentlyFailed = "permanently_failed";
}
