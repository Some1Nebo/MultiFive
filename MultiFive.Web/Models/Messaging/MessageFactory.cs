using System;
using System.Web.Helpers;
using MultiFive.Web.Infrastructure;

namespace MultiFive.Web.Models.Messaging
{
    public class MessageFactory: IMessageFactory
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public MessageFactory(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public Message CreateJoinedMessage(Guid gameId, int receiverId)
        {
            var content = new
            {
                messageName = "joined",
                messageData = new { gameId }
            };

            return new Message(gameId, receiverId, Json.Encode(content), _dateTimeProvider.Now);
        }
    }
}