using CommunicationService.Application.Interfaces;
using Scriban;

namespace CommunicationService.Application.Services;

public class TemplateRenderer : ITemplateRenderer
{
    public async Task<string> RenderAsync(string templateBody, Dictionary<string, object> context)
    {
        var template = Template.Parse(templateBody);
        
        if (template.HasErrors)
        {
            throw new InvalidOperationException($"Template inválido: {string.Join(", ", template.Messages)}");
        }

        var result = await template.RenderAsync(context);
        return result;
    }
}
