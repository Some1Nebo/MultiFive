namespace MultiFive.Domain
{
    public class Player
    {
        public Player(int id)
        {
            Id = id;
        }

        public int Id { get; private set; }
    }
}