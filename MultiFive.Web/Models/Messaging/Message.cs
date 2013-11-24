using System;

namespace MultiFive.Web.Models.Messaging
{
    public class Message
    {
        private Message()
        {
        }

        public Message(Guid gameId, int senderId, string jsonContent,  DateTime creationTime)
        {
            if (string.IsNullOrEmpty(jsonContent))
                throw new ArgumentNullException("jsonContent"); 

            GameId = gameId;
            SenderId = senderId;
            JsonContent = jsonContent; 

            CreationTime = creationTime;
            Status = Status.Unfulfilled;
        }

        public int Id { get; private set; }

        public Guid GameId { get; private set; }
        public int SenderId { get; private set; }
        public string JsonContent { get; private set; }

        public DateTime CreationTime { get; private set; }
        public Status Status { get; private set; }
    }       
}