using PolyTest.Fluent;

namespace PolyTest.Implementations.Fluent
{
    internal class TestRootFluentWrapper<T> : TestCompositeFluentWrapperBase<T>, ITestRootFluent<T>
    {
        public TestRootFluentWrapper(ITestComposite<T> wrapped) : base(wrapped)
        {
        }

        public ITestRootFluent<T> IncludeSelf(string reason = null)
        {
            this.IncludeSelfInEnumeration = true;
            return this;
        }
    }
}