using System;

namespace MultiFive.Domain
{
    public enum Cell
    {
        Empty = 0,
        Player1, 
        Player2
    }

    public class Game
    {
        public enum State
        {
            NotStarted,
            Player1Move,
            Player2Move,
            Player1Win,
            Player2Win,
            Draw
        }

        private Game()
        {
        }

        public Game(int width, int height, Player player1)
        {
            if (player1 == null) 
                throw new ArgumentNullException("player1");

            Id = Guid.NewGuid();
            Player1 = player1;
            Player2 = null;
            CurrentState = State.NotStarted;

            Width = width;
            Height = height;
            FieldData = new byte[Width * Height];
        }

        public void Lock(Player player2, Player startingPlayer = null)
        {
            if (Player2 != null)
                throw new InvalidOperationException("Game is already locked to two players");

            Player2 = player2;

            if (startingPlayer == null)
                startingPlayer = Player1;

            if (startingPlayer.Id != Player1.Id && startingPlayer.Id != Player2.Id)
                throw new ArgumentException(string.Format("StartingPlayer must be either player ({0}) or player ({1}), but was player ({2}).",
                    Player1.Id, Player2.Id, startingPlayer.Id));

            CurrentState = startingPlayer.Id == Player1.Id
                             ? State.Player1Move
                             : State.Player2Move;

        }

        public Cell this[int row, int column]
        {
            get
            {
                var idx = LinearIndex(row, column);
                return (Cell)FieldData[idx];
            }

            private set
            {
                var idx = LinearIndex(row, column);
                FieldData[idx] = (byte)value;
            }
        }

        public void Move(Player player, int row, int column)
        {
            if (player == Player1)
            {
                if (CurrentState == State.Player1Move)
                {
                    this[row, column] = Cell.Player1;
                }
                else
                {
                    var msg = string.Format("Invalid move for game state {0}", CurrentState);
                    throw new InvalidOperationException(msg);
                }
            }
            else if (player == Player2)
            {
                if (CurrentState == State.Player2Move)
                {
                    this[row, column] = Cell.Player2;
                }
                else
                {
                    var msg = string.Format("Invalid move for game state {0}", CurrentState);
                    throw new InvalidOperationException(msg);
                }

            }
            else
            {
                var msg = string.Format("Player must be either player ({0}) or player ({1}), but was player ({2}).",
                    Player1.Id, Player2.Id, player.Id);

                throw new ArgumentException(msg);
            }
        }

        private int LinearIndex(int row, int column)
        {
            return row * Width + column;
        }

        public Guid Id { get; private set; }
        public Player Player1 { get; private set; }
        public Player Player2 { get; private set; }
        public State CurrentState { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public byte[] FieldData { get; private set; }
    }
}