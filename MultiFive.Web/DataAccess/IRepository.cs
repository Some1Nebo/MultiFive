using System;
using System.Collections.Generic;
using System.Linq;
using MultiFive.Domain;
using MultiFive.Web.Models.Messaging;

namespace MultiFive.Web.DataAccess
{
    public interface IRepository
    {
        void Save();

        IQueryable<Game> Games { get;}
        IQueryable<Player> Players { get; }
        IQueryable<Message> Messages { get; }

        Game CreateGame(Player player1);
        Player CreatePlayer();
        
        Player FindPlayer(string userId);

        void AddMessage(Message message);
        IReadOnlyCollection<Message> PollMessages(Guid? channelId, int? pollerId, int lastId);
    }
}