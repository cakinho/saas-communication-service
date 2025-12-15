namespace CommunicationService.Domain.Interfaces;

public interface IChannelSender
{
    string Channel { get; }
    Task<SendResult> SendAsync(SendMessageRequest request);
}

public class SendMessageRequest
{
    public string TenantId { get; set; } = string.Empty;
    public string Recipient { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class SendResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ExternalId { get; set; }

    public static SendResult Ok(string? externalId = null) => new() { Success = true, ExternalId = externalId };
    public static SendResult Fail(string error) => new() { Success = false, ErrorMessage = error };
}
