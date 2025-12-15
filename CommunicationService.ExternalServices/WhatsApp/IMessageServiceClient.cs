using Refit;

namespace CommunicationService.ExternalServices.WhatsApp;

public interface IMessageServiceClient
{
    [Post("/api/wuzapi/chat/send/text")]
    Task<SendTextResponse> SendTextAsync([Body] SendTextRequest request);
}

public class SendTextRequest
{
    [System.Text.Json.Serialization.JsonPropertyName("phone")]
    public string Phone { get; set; } = string.Empty;

    [System.Text.Json.Serialization.JsonPropertyName("body")]
    public string Body { get; set; } = string.Empty;
}

public class SendTextResponse
{
    [System.Text.Json.Serialization.JsonPropertyName("code")]
    public int Code { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("success")]
    public bool Success { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("data")]
    public SendTextResponseData? Data { get; set; }
}

public class SendTextResponseData
{
    [System.Text.Json.Serialization.JsonPropertyName("Details")]
    public string? Details { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("Id")]
    public string? Id { get; set; }
}
