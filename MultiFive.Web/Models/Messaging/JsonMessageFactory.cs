﻿using System;
using System.Web.Helpers;
using MultiFive.Domain;

namespace MultiFive.Web.Models.Messaging
{
    public class JsonMessageFactory: IMessageFactory
    {
        public Message CreateJoinedMessage(Guid gameId, string joinedPlayerName, string gameState)
        {
            return CreateMessage(gameId, null, "joined", new { joinedPlayerName, gameState }); 
        }

        private Message CreateMessage<T>(Guid? channelId, int? receiverId, string name, T content)
        {
            return new Message(channelId, receiverId, name, Json.Encode(content));
        }
    }
}