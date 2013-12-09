using System;
using MultiFive.Domain;

namespace MultiFive.Web.Models.Messaging
{
    public interface IMessageFactory
    {
        Message CreateJoinedMessage(Guid gameId, string joinedPlayerName, string gameState);
    }
}