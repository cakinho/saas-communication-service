using CommunicationService.Domain.Entities;
using CommunicationService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CommunicationService.Persistence.Repositories;

public class MessageTemplateRepository : IMessageTemplateRepository
{
    private readonly CommunicationDbContext _context;

    public MessageTemplateRepository(CommunicationDbContext context)
    {
        _context = context;
    }

    public async Task<MessageTemplate?> GetByKeyAsync(string templateKey, string channel, string? tenantId = null)
    {
        return await _context.MessageTemplates
            .Where(t => t.TemplateKey == templateKey 
                     && t.Channel == channel 
                     && t.TenantId == tenantId 
                     && t.Active)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<MessageTemplate>> GetAllAsync(string? tenantId = null)
    {
        return await _context.MessageTemplates
            .Where(t => t.TenantId == tenantId || t.TenantId == null)
            .Where(t => t.Active)
            .OrderBy(t => t.TemplateKey)
            .ToListAsync();
    }

    public async Task<MessageTemplate> AddAsync(MessageTemplate template)
    {
        _context.MessageTemplates.Add(template);
        await _context.SaveChangesAsync();
        return template;
    }

    public async Task UpdateAsync(MessageTemplate template)
    {
        template.UpdatedAt = DateTime.UtcNow;
        _context.MessageTemplates.Update(template);
        await _context.SaveChangesAsync();
    }
}
