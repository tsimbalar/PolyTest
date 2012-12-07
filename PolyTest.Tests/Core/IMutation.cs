namespace PolyTest.Tests
{
    public interface IMutation<T>
    {
        string Description { get; }
        void Apply(T source);
    }
}