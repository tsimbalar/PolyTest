namespace PolyTest.Tree.Fluent
{
    public interface ITestCompositeNestedFluent<T> : ITestCompositeFluent<T>
    {
        ITestCompositeNestedFluent<T> IgnoreSelf();
        ITestCompositeNestedFluent<T> IncludeSelf();
    }
}