using PolyTest.Fluent;

namespace PolyTest.Implementations.Fluent
{
    internal class TestCompositeFluentNestedWrapper<T> : TestCompositeFluentWrapper<T>, ITestCompositeNestedFluent<T>
    {
        public TestCompositeFluentNestedWrapper(ITestComposite<T> wrapped)
            : base(wrapped)
        {
        }

        public ITestCompositeNestedFluent<T> IgnoreSelf(string reason = null)
        {
            this.IncludeSelfInEnumeration = false;
            return this;
        }
    }
}
