using System;
using System.Web.Helpers;
using MultiFive.Domain;

namespace MultiFive.Web.Models.Messaging
{
    public class JsonMessageFactory: IMessageFactory
    {
        public Message CreateJoinedMessage(Guid gameId, string joinedPlayerName, Game.State gameState)
        {
            return CreateMessage(gameId, null, "joined", new
            {
                joinedPlayerName, 
                gameState = gameState.ToString()
            }); 
        }

        public Message CreateMovedMessage(Guid gameId, PlayerRole playerRole, int row, int col, Game.State gameState)
        {
            return CreateMessage(gameId, null, "moved", new
            {
                playerRole = playerRole.ToString(),
                row,
                col,
                gameState = gameState.ToString()
            });
        }

        private Message CreateMessage<T>(Guid? channelId, int? receiverId, string name, T content)
        {
            return new Message(channelId, receiverId, name, Json.Encode(content));
        }
    }
}