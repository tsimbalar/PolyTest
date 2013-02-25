namespace PolyTest.Fluent
{
    public interface ITestCompositeNestedFluent<T> : ITestCompositeFluent<T>
    {
        ITestCompositeNestedFluent<T> IgnoreSelf(string reason = null);
        //ITestCompositeNestedFluent<T> IncludeSelf(string reason = null);
    }
}