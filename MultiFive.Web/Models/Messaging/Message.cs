using System;

namespace MultiFive.Web.Models.Messaging
{
    public class Message
    {
        private Message()
        {
        }

        public Message(Guid gameId, int receiverId, string jsonContent, DateTime creationTime)
        {
            if (string.IsNullOrEmpty(jsonContent))
                throw new ArgumentNullException("jsonContent"); 

            GameId = gameId;
            ReceiverId = receiverId;
            JsonContent = jsonContent; 

            CreationTime = creationTime;
            Status = Status.Unfulfilled;
        }

        public int Id { get; private set; }

        public Guid GameId { get; private set; }
        public int ReceiverId { get; private set; }
        public string JsonContent { get; private set; }

        public Status Status { get; set; }
        public DateTime CreationTime { get; private set; }
    }       
}