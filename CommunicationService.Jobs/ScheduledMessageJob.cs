using CommunicationService.Domain.Entities;
using CommunicationService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace CommunicationService.Jobs;

public class ScheduledMessageJob
{
    private readonly IScheduledMessageRepository _scheduledMessageRepository;
    private readonly ILogger<ScheduledMessageJob> _logger;

    public ScheduledMessageJob(
        IScheduledMessageRepository scheduledMessageRepository,
        ILogger<ScheduledMessageJob> logger)
    {
        _scheduledMessageRepository = scheduledMessageRepository;
        _logger = logger;
    }

    public async Task ProcessPendingMessagesAsync()
    {
        _logger.LogInformation("Iniciando processamento de mensagens agendadas");

        var pendingMessages = await _scheduledMessageRepository.GetPendingMessagesAsync(DateTime.UtcNow);
        var count = 0;

        foreach (var scheduled in pendingMessages)
        {
            try
            {
                // TODO: Implementar lógica de envio baseada no EventId
                // Por enquanto, apenas marca como enviado
                scheduled.Status = ScheduledMessageStatus.Sent;
                scheduled.SentAt = DateTime.UtcNow;
                await _scheduledMessageRepository.UpdateAsync(scheduled);

                count++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem agendada {Id}", scheduled.Id);
            }
        }

        _logger.LogInformation("Processamento concluído. {Count} mensagens processadas", count);
    }
}
