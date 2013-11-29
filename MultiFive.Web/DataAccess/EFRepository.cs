using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using MultiFive.Domain;
using MultiFive.Web.Models.Messaging;

namespace MultiFive.Web.DataAccess
{
    public class EFRepository: IRepository
    {
        private readonly ApplicationDbContext _dbContext; 

        public EFRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext; 
        }

        public IQueryable<Game> Games
        {
            get { return _dbContext.Games; }
        }

        public IQueryable<Player> Players
        {
            get { return _dbContext.Players; }
        }

        public IQueryable<Message> Messages
        {
            get { return _dbContext.Messages; }
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public Game CreateGame(Player player1)
        {
            var game = new Game(player1);

            return _dbContext.Games.Add(game);
        }

        public Player CreatePlayer()
        {
            var player = _dbContext.Players.Create();

            return _dbContext.Players.Add(player);
        }

        public Player FindPlayer(string userId)
        {
            var user = _dbContext.Users.Include(u => u.Player).FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                string msg = string.Format("User with id {0} not found in the db.", userId);
                throw new ApplicationException(msg);
            }

            if (user.Player == null)
            {
                string msg = string.Format("This shouldn't happen. Player associated with user {0} not found.", userId);
                throw new ApplicationException(msg);
            }

            return user.Player;
        }

        public void AddMessage(Message message)
        {
            _dbContext.Messages.Add(message); 
        }

        public IReadOnlyCollection<Message> PollMessages(Guid gameId, int receiverId)
        {
            List<Message> messages;

            using (var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                messages = Messages
                    .Where(m => m.ReceiverId == receiverId
                                && m.GameId == gameId
                                && m.Status != Status.Fulfilled)
                    .OrderBy(m => m.CreationTime)
                    .ToList();

                messages.ForEach(m => m.Status = Status.Fulfilled);

                _dbContext.SaveChanges();

                transaction.Commit();
            }

            return messages;
        }
    }
}