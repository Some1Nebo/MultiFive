namespace MultiFive.Domain
{
    public interface IObjectWithIncrementalState
    {
        int StateNumber { get; set; }
    }
}