using CommunicationService.Domain.Entities;
using CommunicationService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CommunicationService.Persistence.Repositories;

public class ScheduledMessageRepository : IScheduledMessageRepository
{
    private readonly CommunicationDbContext _context;

    public ScheduledMessageRepository(CommunicationDbContext context)
    {
        _context = context;
    }

    public async Task<ScheduledMessage?> GetByIdAsync(string id)
    {
        return await _context.ScheduledMessages.FindAsync(id);
    }

    public async Task<IEnumerable<ScheduledMessage>> GetPendingMessagesAsync(DateTime until)
    {
        return await _context.ScheduledMessages
            .Where(m => m.Status == ScheduledMessageStatus.Pending && m.ScheduledAt <= until)
            .OrderBy(m => m.ScheduledAt)
            .Take(100)
            .ToListAsync();
    }

    public async Task<IEnumerable<ScheduledMessage>> GetBySourceEntityIdAsync(string sourceEntityId)
    {
        return await _context.ScheduledMessages
            .Where(m => m.SourceEntityId == sourceEntityId)
            .ToListAsync();
    }

    public async Task<ScheduledMessage> AddAsync(ScheduledMessage scheduledMessage)
    {
        _context.ScheduledMessages.Add(scheduledMessage);
        await _context.SaveChangesAsync();
        return scheduledMessage;
    }

    public async Task UpdateAsync(ScheduledMessage scheduledMessage)
    {
        _context.ScheduledMessages.Update(scheduledMessage);
        await _context.SaveChangesAsync();
    }

    public async Task CancelBySourceEntityIdAsync(string sourceEntityId)
    {
        var now = DateTime.UtcNow;
        await _context.ScheduledMessages
            .Where(m => m.SourceEntityId == sourceEntityId && m.Status == ScheduledMessageStatus.Pending)
            .ExecuteUpdateAsync(s => s
                .SetProperty(m => m.Status, ScheduledMessageStatus.Cancelled)
                .SetProperty(m => m.CancelledAt, now));
    }
}
