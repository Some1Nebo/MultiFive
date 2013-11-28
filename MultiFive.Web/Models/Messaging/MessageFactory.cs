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

        public Message CreateMessage<T>(Guid gameId, int receiverId, string messageName, T messageData)
        {
            var message = new
            {
                messageName,
                messageData
            };

            return new Message(gameId, receiverId, Json.Encode(message), _dateTimeProvider.Now);
        }
    }
}