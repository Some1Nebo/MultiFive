using System;
using System.Web.Script.Serialization;
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

            var jsonSerializer = new JavaScriptSerializer();
            var jsonContent = jsonSerializer.Serialize(content);

            return new Message(gameId, receiverId, jsonContent, _dateTimeProvider.Now);
        }
    }
}