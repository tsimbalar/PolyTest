namespace PolyTest.Fluent
{
    public interface ITestCompositeNestedFluent<T> : ITestCompositeFluent<T>
    {
        ITestCompositeNestedFluent<T> IgnoreSelf(string reason = null);
    }
}