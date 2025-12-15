using CommunicationService.Domain.Entities;
using CommunicationService.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CommunicationService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TemplatesController : ControllerBase
{
    private readonly IMessageTemplateRepository _templateRepository;

    public TemplatesController(IMessageTemplateRepository templateRepository)
    {
        _templateRepository = templateRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? tenantId = null)
    {
        var templates = await _templateRepository.GetAllAsync(tenantId);
        return Ok(templates);
    }

    [HttpGet("{templateKey}/{channel}")]
    public async Task<IActionResult> GetByKey(string templateKey, string channel, [FromQuery] string? tenantId = null)
    {
        var template = await _templateRepository.GetByKeyAsync(templateKey, channel, tenantId);
        
        if (template == null)
            return NotFound();

        return Ok(template);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTemplateRequest request)
    {
        var template = new MessageTemplate
        {
            TenantId = request.TenantId,
            TemplateKey = request.TemplateKey,
            Channel = request.Channel,
            TemplateBody = request.TemplateBody,
            Variables = request.Variables,
            Active = true
        };

        await _templateRepository.AddAsync(template);
        return CreatedAtAction(nameof(GetByKey), new { templateKey = template.TemplateKey, channel = template.Channel }, template);
    }
}

public class CreateTemplateRequest
{
    public string? TenantId { get; set; }
    public string TemplateKey { get; set; } = string.Empty;
    public string Channel { get; set; } = "whatsapp";
    public string TemplateBody { get; set; } = string.Empty;
    public string? Variables { get; set; }
}
