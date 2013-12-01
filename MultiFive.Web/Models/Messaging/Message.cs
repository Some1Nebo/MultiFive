using System;

namespace MultiFive.Web.Models.Messaging
{
    public class Message
    {
        private Message()
        {
        }

        public Message(Guid? channelId, int? receiverId, string name, string content)
        {
            ChannelId = channelId;
            ReceiverId = receiverId;
            Name = name;
            Content = content;
        }

        public Guid? ChannelId { get; private set; }
        public int? ReceiverId { get; private set; }
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Content { get; private set; }
    }
}