using CommunicationService.Application.Interfaces;
using CommunicationService.Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CommunicationService.Infrastructure.Consumers;

public class CancelScheduledMessagesConsumer : IConsumer<CancelScheduledMessages>
{
    private readonly ICommunicationService _communicationService;
    private readonly ILogger<CancelScheduledMessagesConsumer> _logger;

    public CancelScheduledMessagesConsumer(
        ICommunicationService communicationService,
        ILogger<CancelScheduledMessagesConsumer> logger)
    {
        _communicationService = communicationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CancelScheduledMessages> context)
    {
        var request = context.Message;
        
        _logger.LogInformation(
            "Recebido CancelScheduledMessages {EventId} para {SourceEntityId}",
            request.EventId, request.SourceEntityId);

        try
        {
            await _communicationService.CancelScheduledMessagesAsync(request.SourceEntityId);
            
            _logger.LogInformation(
                "Mensagens agendadas canceladas para {SourceEntityId}",
                request.SourceEntityId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao cancelar mensagens agendadas {EventId}", request.EventId);
            throw;
        }
    }
}
