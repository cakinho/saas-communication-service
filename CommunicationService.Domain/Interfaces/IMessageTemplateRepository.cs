using CommunicationService.Domain.Entities;

namespace CommunicationService.Domain.Interfaces;

public interface IMessageTemplateRepository
{
    Task<MessageTemplate?> GetByKeyAsync(string templateKey, string channel, string? tenantId = null);
    Task<IEnumerable<MessageTemplate>> GetAllAsync(string? tenantId = null);
    Task<MessageTemplate> AddAsync(MessageTemplate template);
    Task UpdateAsync(MessageTemplate template);
}
