using CommunicationService.Application.Interfaces;
using CommunicationService.Contracts.Events;
using CommunicationService.Domain.Entities;
using CommunicationService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CommunicationService.Application.Services;

public class CommunicationAppService : ICommunicationService
{
    private readonly IMessageTemplateRepository _templateRepository;
    private readonly IMessageLogRepository _messageLogRepository;
    private readonly IScheduledMessageRepository _scheduledMessageRepository;
    private readonly IChannelSenderFactory _channelSenderFactory;
    private readonly ITemplateRenderer _templateRenderer;
    private readonly ILogger<CommunicationAppService> _logger;

    public CommunicationAppService(
        IMessageTemplateRepository templateRepository,
        IMessageLogRepository messageLogRepository,
        IScheduledMessageRepository scheduledMessageRepository,
        IChannelSenderFactory channelSenderFactory,
        ITemplateRenderer templateRenderer,
        ILogger<CommunicationAppService> logger)
    {
        _templateRepository = templateRepository;
        _messageLogRepository = messageLogRepository;
        _scheduledMessageRepository = scheduledMessageRepository;
        _channelSenderFactory = channelSenderFactory;
        _templateRenderer = templateRenderer;
        _logger = logger;
    }

    public async Task<string> ProcessMessageAsync(CommunicationRequest request)
    {
        _logger.LogInformation("Processando mensagem {EventId} do {Source}", request.EventId, request.Source);

        if (request.Options.ScheduleAt.HasValue && request.Options.ScheduleAt > DateTime.UtcNow)
        {
            return await ScheduleMessageAsync(request);
        }

        return await SendMessageAsync(request);
    }

    private async Task<string> ScheduleMessageAsync(CommunicationRequest request)
    {
        var scheduled = new ScheduledMessage
        {
            TenantId = request.TenantId,
            SourceEntityId = request.Options.SourceEntityId ?? request.EventId,
            MessageType = "scheduled",
            ScheduledAt = request.Options.ScheduleAt!.Value
        };

        await _scheduledMessageRepository.AddAsync(scheduled);
        _logger.LogInformation("Mensagem agendada {Id} para {ScheduledAt}", scheduled.Id, scheduled.ScheduledAt);
        
        return scheduled.Id;
    }

    private async Task<string> SendMessageAsync(CommunicationRequest request)
    {
        var recipient = GetRecipient(request);
        
        var template = await _templateRepository.GetByKeyAsync(request.TemplateKey, request.Channel, request.TenantId)
                    ?? await _templateRepository.GetByKeyAsync(request.TemplateKey, request.Channel);

        if (template == null)
        {
            _logger.LogError("Template não encontrado: {TemplateKey} para canal {Channel}", request.TemplateKey, request.Channel);
            throw new InvalidOperationException($"Template '{request.TemplateKey}' não encontrado para canal '{request.Channel}'");
        }

        var messageBody = await _templateRenderer.RenderAsync(template.TemplateBody, request.Context);

        // ============================================================
        // MODO SIMPLIFICADO: Sem salvar no banco, retry via RabbitMQ
        // Para voltar ao modo com banco, descomente o bloco abaixo
        // ============================================================
        
        var sender = _channelSenderFactory.GetSender(request.Channel);
        var result = await sender.SendAsync(new SendMessageRequest
        {
            TenantId = request.TenantId,
            Recipient = recipient,
            Message = messageBody
        });

        if (result.Success)
        {
            _logger.LogInformation("✅ Mensagem enviada com sucesso para {Recipient}", recipient);
            return request.EventId;
        }
        else
        {
            _logger.LogWarning("❌ Falha ao enviar mensagem para {Recipient}: {Error}", recipient, result.ErrorMessage);
            throw new InvalidOperationException($"Falha ao enviar mensagem: {result.ErrorMessage}");
        }

        // ============================================================
        // CÓDIGO ORIGINAL COM BANCO (comentado para uso futuro)
        // ============================================================
        /*
        var messageLog = new MessageLog
        {
            TenantId = request.TenantId,
            Channel = request.Channel,
            Recipient = recipient,
            TemplateKey = request.TemplateKey,
            MessageBody = messageBody,
            Status = MessageStatus.Pending
        };

        await _messageLogRepository.AddAsync(messageLog);

        try
        {
            var sender = _channelSenderFactory.GetSender(request.Channel);
            var result = await sender.SendAsync(new SendMessageRequest
            {
                TenantId = request.TenantId,
                Recipient = recipient,
                Message = messageBody
            });

            if (result.Success)
            {
                messageLog.Status = MessageStatus.Sent;
                messageLog.SentAt = DateTime.UtcNow;
                _logger.LogInformation("Mensagem {Id} enviada com sucesso", messageLog.Id);
            }
            else
            {
                messageLog.Status = MessageStatus.Failed;
                messageLog.ErrorMessage = result.ErrorMessage;
                messageLog.RetryCount = 1;
                _logger.LogWarning("Falha ao enviar mensagem {Id}: {Error}", messageLog.Id, result.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            messageLog.Status = MessageStatus.Failed;
            messageLog.ErrorMessage = ex.Message;
            messageLog.RetryCount = 1;
            _logger.LogError(ex, "Erro ao enviar mensagem {Id}", messageLog.Id);
        }

        await _messageLogRepository.UpdateAsync(messageLog);
        return messageLog.Id;
        */
    }

    public async Task CancelScheduledMessagesAsync(string sourceEntityId)
    {
        await _scheduledMessageRepository.CancelBySourceEntityIdAsync(sourceEntityId);
        _logger.LogInformation("Mensagens agendadas canceladas para {SourceEntityId}", sourceEntityId);
    }

    private static string GetRecipient(CommunicationRequest request)
    {
        return request.Channel.ToLower() switch
        {
            "whatsapp" or "sms" => request.Recipient.Phone ?? throw new ArgumentException("Phone é obrigatório para WhatsApp/SMS"),
            "email" => request.Recipient.Email ?? throw new ArgumentException("Email é obrigatório para Email"),
            _ => request.Recipient.Phone ?? request.Recipient.Email ?? throw new ArgumentException("Recipient não informado")
        };
    }
}
