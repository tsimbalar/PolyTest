using System;

namespace PolyTest.Implementations
{
    /// <summary>
    /// The bottom element of the tree, a test case with no children
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class TestLeaf<T> : TestNodeBase<T>
    {
        private readonly IMutation<T> _mutation;

        public TestLeaf(ITestComposite<T> parent, IMutation<T> mutation)
            : base(parent)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (mutation == null) throw new ArgumentNullException("mutation");
            _mutation = mutation;
        }

        protected override string NodeDescription
        {
            get { return _mutation.Description; }
        }

        public override bool IncludeSelfInEnumeration
        {
            get { return true; }
        }

        public override T Arrange()
        {
            var startFrom = Parent.Arrange();
            _mutation.Apply(startFrom);
            return startFrom;
        }
    }
}
