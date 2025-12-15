using CommunicationService.Application.Interfaces;
using CommunicationService.Domain.Interfaces;

namespace CommunicationService.ExternalServices;

public class ChannelSenderFactory : IChannelSenderFactory
{
    private readonly Dictionary<string, IChannelSender> _senders;

    public ChannelSenderFactory(IEnumerable<IChannelSender> senders)
    {
        _senders = senders.ToDictionary(s => s.Channel.ToLower(), s => s);
    }

    public IChannelSender GetSender(string channel)
    {
        var key = channel.ToLower();
        
        if (!_senders.TryGetValue(key, out var sender))
        {
            throw new NotSupportedException($"Canal '{channel}' não suportado. Canais disponíveis: {string.Join(", ", _senders.Keys)}");
        }

        return sender;
    }

    public bool HasSender(string channel)
    {
        return _senders.ContainsKey(channel.ToLower());
    }
}
