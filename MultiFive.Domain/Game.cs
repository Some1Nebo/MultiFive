using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiFive.Domain
{
    public class Game
    {
        public Game(Player player1)
        {
            if (player1 == null) 
                throw new NullReferenceException("player1 can't be null");

            Id = Guid.NewGuid();
            Player1 = player1;
            Player2 = null;
            CurrentPlayer = 0;
        }

        public void Lock(Player player2)
        {
            if (Player2 != null)
                throw new InvalidOperationException("Game is already locked to two players");

            Player2 = player2;
        }

        public void Move()
        {
            CurrentPlayer = (CurrentPlayer + 1)%2;
        }

        public Guid Id { get; private set; }
        public Player Player1 { get; private set; }
        public Player Player2 { get; private set; }
        public int CurrentPlayer { get; private set; }
    }
}
