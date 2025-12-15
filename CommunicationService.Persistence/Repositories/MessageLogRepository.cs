using CommunicationService.Domain.Entities;
using CommunicationService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CommunicationService.Persistence.Repositories;

public class MessageLogRepository : IMessageLogRepository
{
    private readonly CommunicationDbContext _context;

    public MessageLogRepository(CommunicationDbContext context)
    {
        _context = context;
    }

    public async Task<MessageLog?> GetByIdAsync(string id)
    {
        return await _context.MessageLogs.FindAsync(id);
    }

    public async Task<IEnumerable<MessageLog>> GetByEventIdAsync(string eventId)
    {
        return await _context.MessageLogs
            .Where(m => m.EventId == eventId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<MessageLog>> GetFailedMessagesAsync(int maxRetryCount)
    {
        return await _context.MessageLogs
            .Where(m => m.Status == MessageStatus.Failed && m.RetryCount < maxRetryCount)
            .OrderBy(m => m.CreatedAt)
            .Take(100)
            .ToListAsync();
    }

    public async Task<MessageLog> AddAsync(MessageLog messageLog)
    {
        _context.MessageLogs.Add(messageLog);
        await _context.SaveChangesAsync();
        return messageLog;
    }

    public async Task UpdateAsync(MessageLog messageLog)
    {
        _context.MessageLogs.Update(messageLog);
        await _context.SaveChangesAsync();
    }
}
