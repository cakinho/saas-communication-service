using CommunicationService.Contracts.Events;

namespace CommunicationService.Application.Interfaces;

public interface ICommunicationService
{
    Task<string> ProcessMessageAsync(CommunicationRequest request);
    Task CancelScheduledMessagesAsync(string sourceEntityId);
}
