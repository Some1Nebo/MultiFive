using System;

namespace MultiFive.Web.Models.Messaging
{
    public interface IMessageFactory
    {
        Message CreateMessage<T>(Guid gameId, int receiverId, string messageName, T messageData);
    }
}