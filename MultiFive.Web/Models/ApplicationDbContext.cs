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
            var game = Games.Create();
            return Games.Add(game);
        }

        Player IRepository.CreatePlayer()
        {
            var player = Players.Create();
            return Players.Add(player);
        }
    }
}