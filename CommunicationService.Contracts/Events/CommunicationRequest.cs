namespace CommunicationService.Contracts.Events;

/// <summary>
/// Evento genérico para solicitar envio de comunicação.
/// Qualquer microserviço pode publicar este evento.
/// </summary>
public class CommunicationRequest
{
    public string EventId { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Source { get; set; } = string.Empty; // scheduling-service, finance-service, etc.
    public string TenantId { get; set; } = string.Empty;
    
    public string Channel { get; set; } = "whatsapp"; // whatsapp, email, sms, push
    public RecipientInfo Recipient { get; set; } = new();
    public string TemplateKey { get; set; } = string.Empty;
    public Dictionary<string, object> Context { get; set; } = new(); // variáveis do template
    
    public MessageOptions Options { get; set; } = new();
}

public class RecipientInfo
{
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
}

public class MessageOptions
{
    public DateTime? ScheduleAt { get; set; } // null = enviar imediatamente
    public string Priority { get; set; } = "normal"; // high, normal, low
    public string? SourceEntityId { get; set; } // appointmentId, invoiceId, etc.
}
