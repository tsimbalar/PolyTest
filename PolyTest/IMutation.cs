namespace PolyTest
{
    public interface IMutation<in T>
    {
        string Description { get; }
        void Apply(T source);
    }
}