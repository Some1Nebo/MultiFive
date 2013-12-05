namespace MultiFive.Domain
{
    public interface IObjectWithIncrementalState<in TDelta>
    {
        int IncrementNumber { get; }
        void Update(TDelta delta, int incrementNumber);
    }
}