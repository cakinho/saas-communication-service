using CommunicationService.Domain.Entities;

namespace CommunicationService.Domain.Interfaces;

public interface IMessageLogRepository
{
    Task<MessageLog?> GetByIdAsync(string id);
    Task<IEnumerable<MessageLog>> GetByEventIdAsync(string eventId);
    Task<IEnumerable<MessageLog>> GetFailedMessagesAsync(int maxRetryCount);
    Task<MessageLog> AddAsync(MessageLog messageLog);
    Task UpdateAsync(MessageLog messageLog);
}
