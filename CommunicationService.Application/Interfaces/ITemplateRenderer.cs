namespace CommunicationService.Application.Interfaces;

public interface ITemplateRenderer
{
    Task<string> RenderAsync(string templateBody, Dictionary<string, object> context);
}
