using CommunicationService.Domain.Interfaces;

namespace CommunicationService.Application.Interfaces;

public interface IChannelSenderFactory
{
    IChannelSender GetSender(string channel);
    bool HasSender(string channel);
}
