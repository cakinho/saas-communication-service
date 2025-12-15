namespace CommunicationService.Contracts.Events;

/// <summary>
/// Evento para cancelar mensagens agendadas por referência.
/// Útil quando um agendamento é cancelado/alterado.
/// </summary>
public class CancelScheduledMessages
{
    public string EventId { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Source { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    
    public string SourceEntityId { get; set; } = string.Empty;
}
