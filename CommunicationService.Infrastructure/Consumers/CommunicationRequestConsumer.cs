using CommunicationService.Application.Interfaces;
using CommunicationService.Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CommunicationService.Infrastructure.Consumers;

public class CommunicationRequestConsumer : IConsumer<CommunicationRequest>
{
    private readonly ICommunicationService _communicationService;
    private readonly ILogger<CommunicationRequestConsumer> _logger;

    public CommunicationRequestConsumer(
        ICommunicationService communicationService,
        ILogger<CommunicationRequestConsumer> logger)
    {
        _communicationService = communicationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CommunicationRequest> context)
    {
        var request = context.Message;
        
        _logger.LogInformation(
            "Recebido CommunicationRequest {EventId} de {Source} para {Channel}",
            request.EventId, request.Source, request.Channel);

        try
        {
            var messageId = await _communicationService.ProcessMessageAsync(request);
            
            _logger.LogInformation(
                "CommunicationRequest {EventId} processado com sucesso. MessageId: {MessageId}",
                request.EventId, messageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar CommunicationRequest {EventId}", request.EventId);
            throw; // MassTransit vai fazer retry
        }
    }
}
