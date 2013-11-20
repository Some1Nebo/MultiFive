using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiFive.Domain
{
    public class Game
    {
        private readonly List<IPlayer> _players;

        public Game(IPlayer startingPlayer)
        {
            Id = Guid.NewGuid();
            Players = _players = new List<IPlayer> { startingPlayer };
            CurrentPlayer = 0;
            GameFull = _players.Count == MaxPlayers;
        }

        public const int MaxPlayers = 2;
        public Guid Id { get; private set; }
        public IReadOnlyCollection<IPlayer> Players { get; private set; }
        public int CurrentPlayer { get; private set; }

        public void AddPlayer(IPlayer player)
        {
            GameFull = _players.Count < MaxPlayers;

            if (!GameFull)
            {
                _players.Add(player);
            }
            else
            {
                string msg = string.Format("Can't add player. Game already has {0}.", MaxPlayers);
                throw new ArgumentException(msg);
            }
        }

        public bool GameFull { get; private set; }
    }

    public interface IPlayer
    {
        string Id { get; }
    }
}
