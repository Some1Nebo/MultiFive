using System.Data.Entity;
using System.Linq;
using MultiFive.Domain;

namespace MultiFive.Web.Models
{
    public interface IRepository
    {
        void Save();

        IQueryable<Game> Games { get;}
        IQueryable<Player> Players { get; }

        Game CreateGame(Player player1);
        Player CreatePlayer();
        
        Player FindPlayer(string userId);
    }
}