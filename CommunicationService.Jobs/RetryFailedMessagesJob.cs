using CommunicationService.Application.Interfaces;
using CommunicationService.Domain.Entities;
using CommunicationService.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CommunicationService.Jobs;

public class RetryFailedMessagesJob
{
    private readonly IMessageLogRepository _messageLogRepository;
    private readonly IChannelSenderFactory _channelSenderFactory;
    private readonly ILogger<RetryFailedMessagesJob> _logger;
    private readonly RetrySettings _settings;

    public RetryFailedMessagesJob(
        IMessageLogRepository messageLogRepository,
        IChannelSenderFactory channelSenderFactory,
        IOptions<RetrySettings> settings,
        ILogger<RetryFailedMessagesJob> logger)
    {
        _messageLogRepository = messageLogRepository;
        _channelSenderFactory = channelSenderFactory;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task ProcessFailedMessagesAsync()
    {
        _logger.LogInformation("Iniciando retry de mensagens com falha");

        var failedMessages = await _messageLogRepository.GetFailedMessagesAsync(_settings.MaxAttempts);
        var count = 0;

        foreach (var message in failedMessages)
        {
            try
            {
                var sender = _channelSenderFactory.GetSender(message.Channel);
                var result = await sender.SendAsync(new SendMessageRequest
                {
                    TenantId = message.TenantId,
                    Recipient = message.Recipient,
                    Message = message.MessageBody
                });

                if (result.Success)
                {
                    message.Status = MessageStatus.Sent;
                    message.SentAt = DateTime.UtcNow;
                    _logger.LogInformation("Retry bem sucedido para mensagem {Id}", message.Id);
                }
                else
                {
                    message.RetryCount++;
                    message.ErrorMessage = result.ErrorMessage;
                    
                    if (message.RetryCount >= _settings.MaxAttempts)
                    {
                        message.Status = MessageStatus.PermanentlyFailed;
                        _logger.LogWarning("Mensagem {Id} marcada como permanentemente falha após {Attempts} tentativas", 
                            message.Id, message.RetryCount);
                    }
                }

                await _messageLogRepository.UpdateAsync(message);
                count++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no retry da mensagem {Id}", message.Id);
            }
        }

        _logger.LogInformation("Retry concluído. {Count} mensagens processadas", count);
    }
}

public class RetrySettings
{
    public int MaxAttempts { get; set; } = 3;
    public int DelayMinutes { get; set; } = 5;
}
