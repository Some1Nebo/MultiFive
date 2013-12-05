namespace MultiFive.Domain
{
    public class GameMove
    {
        public GameMove(Player player, int row, int column)
        {
            Column = column;
            Row = row;
            Player = player;
        }

        public Player Player { get; private set; }
        public int Row { get; private set; }
        public int Column { get; private set; }
    }
}