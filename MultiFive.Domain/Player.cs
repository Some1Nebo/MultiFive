namespace MultiFive.Domain
{
    public class Player
    {
        private Player()
        {
        }

        public Player(int id)
        {
            Id = id;
        }

        public int Id { get; private set; }
    }
}