using System.Data.Entity;
using System.Linq;
using MultiFive.Domain;

namespace MultiFive.Web.Models
{
    public interface IRepository
    {
        IQueryable<Game> Games { get;}
        IQueryable<Player> Players { get; }

        Game CreateGame(Player player1);
        Player CreatePlayer();
        
        void Save();
    }
}