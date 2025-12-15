namespace CommunicationService.Domain.Entities;

public class MessageTemplate
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? TenantId { get; set; } // null = template global
    public string TemplateKey { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty; // whatsapp, email, sms
    public string TemplateBody { get; set; } = string.Empty;
    public string? Variables { get; set; } // JSON array: ["clientName", "scheduledAt"]
    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
