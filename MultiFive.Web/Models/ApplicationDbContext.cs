using System;
using System.Data.Entity;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;
using MultiFive.Domain;

namespace MultiFive.Web.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IRepository
    {
        public ApplicationDbContext()
            : base("MultiFiveDb")
        {
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<Player> Players { get; set; }

        IQueryable<Game> IRepository.Games
        {
            get { return Games; }
        }

        IQueryable<Player> IRepository.Players
        {
            get { return Players; }
        }

        void IRepository.Save()
        {
            SaveChanges();
        }

        Game IRepository.CreateGame(Player player1)
        {
            var game = new Game(player1);
            return Games.Add(game);
        }

        Player IRepository.CreatePlayer()
        {
            var player = Players.Create();
            return Players.Add(player);
        }

        Player IRepository.FindPlayer(string userId)
        {
            var user = Users.Include(u => u.Player).FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                string msg = string.Format("User with id {0} not found in the db.", userId);
                throw new ApplicationException(msg);
            }

            if (user.Player == null)
            {
                string msg = string.Format("This shouldn't happen. Player associated with user {0} not found.");
                throw new ApplicationException(msg);
            }

            return user.Player;
        }
    }
}