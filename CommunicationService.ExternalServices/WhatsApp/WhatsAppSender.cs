using CommunicationService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CommunicationService.ExternalServices.WhatsApp;

public class WhatsAppSender : IChannelSender
{
    private readonly IMessageServiceClient _messageServiceClient;
    private readonly ILogger<WhatsAppSender> _logger;

    public string Channel => "whatsapp";

    public WhatsAppSender(IMessageServiceClient messageServiceClient, ILogger<WhatsAppSender> logger)
    {
        _messageServiceClient = messageServiceClient;
        _logger = logger;
    }

    public async Task<SendResult> SendAsync(SendMessageRequest request)
    {
        try
        {
            _logger.LogInformation("Enviando WhatsApp para {Recipient}", request.Recipient);

            var response = await _messageServiceClient.SendTextAsync(new SendTextRequest
            {
                Phone = request.Recipient,
                Body = request.Message
            });

            if (response.Success)
            {
                _logger.LogInformation("WhatsApp enviado com sucesso para {Recipient}", request.Recipient);
                return SendResult.Ok(response.Data?.Id);
            }

            _logger.LogWarning("Falha ao enviar WhatsApp para {Recipient}: Code {Code}", request.Recipient, response.Code);
            return SendResult.Fail($"Erro ao enviar: Code {response.Code}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar WhatsApp para {Recipient}", request.Recipient);
            return SendResult.Fail(ex.Message);
        }
    }
}
