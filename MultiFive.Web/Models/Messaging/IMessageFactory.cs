using System;
using MultiFive.Domain;

namespace MultiFive.Web.Models.Messaging
{
    public interface IMessageFactory
    {
        Message CreateJoinedMessage(Guid gameId, string joinedPlayerName, Game.State gameState);
        Message CreateMovedMessage(Guid gameId, PlayerRole playerRole, int row, int col, Game.State gameState);
    }
}