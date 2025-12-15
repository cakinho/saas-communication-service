using CommunicationService.Domain.Entities;

namespace CommunicationService.Domain.Interfaces;

public interface IScheduledMessageRepository
{
    Task<ScheduledMessage?> GetByIdAsync(string id);
    Task<IEnumerable<ScheduledMessage>> GetPendingMessagesAsync(DateTime until);
    Task<IEnumerable<ScheduledMessage>> GetBySourceEntityIdAsync(string sourceEntityId);
    Task<ScheduledMessage> AddAsync(ScheduledMessage scheduledMessage);
    Task UpdateAsync(ScheduledMessage scheduledMessage);
    Task CancelBySourceEntityIdAsync(string sourceEntityId);
}
