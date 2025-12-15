using CommunicationService.Application.Interfaces;
using CommunicationService.Contracts.Events;
using CommunicationService.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CommunicationService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly ICommunicationService _communicationService;
    private readonly IMessageLogRepository _messageLogRepository;

    public MessagesController(
        ICommunicationService communicationService,
        IMessageLogRepository messageLogRepository)
    {
        _communicationService = communicationService;
        _messageLogRepository = messageLogRepository;
    }

    /// <summary>
    /// Envia uma mensagem diretamente (sem passar pelo RabbitMQ).
    /// </summary>
    [HttpPost("send")]
    public async Task<IActionResult> Send([FromBody] CommunicationRequest request)
    {
        try
        {
            var messageId = await _communicationService.ProcessMessageAsync(request);
            return Ok(new { success = true, messageId });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    /// <summary>
    /// Busca logs de mensagens por EventId.
    /// </summary>
    [HttpGet("logs/event/{eventId}")]
    public async Task<IActionResult> GetLogsByEvent(string eventId)
    {
        var logs = await _messageLogRepository.GetByEventIdAsync(eventId);
        return Ok(logs);
    }

    /// <summary>
    /// Busca um log específico por ID.
    /// </summary>
    [HttpGet("logs/{id}")]
    public async Task<IActionResult> GetLog(string id)
    {
        var log = await _messageLogRepository.GetByIdAsync(id);
        
        if (log == null)
            return NotFound();

        return Ok(log);
    }
}
