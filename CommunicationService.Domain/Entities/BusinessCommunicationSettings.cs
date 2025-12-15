namespace CommunicationService.Domain.Entities;

public class BusinessCommunicationSettings
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenantId { get; set; } = string.Empty;
    public string Settings { get; set; } = "{}"; // JSON com configurações
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
