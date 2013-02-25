using System;

namespace PolyTest.Implementations
{
    /// <summary>
    /// TestComposite, that is, a node with a collection of nodes, where each child node provide a mutation to apply to the parent
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class TestComposite<T> : TestCompositeBase<T>
    {
        private readonly IMutation<T> _mutation;

        /// <summary>
        /// Creates a TestComposite 
        /// </summary>
        /// <param name="parent">the parent for this component</param>
        /// <param name="mutation">which mutation this element introduces</param>
        public TestComposite(ITestComposite<T> parent, IMutation<T> mutation)
            : base(parent, true /*include itself in enumeration - to be ignored later if necessary*/)
        {
            if (parent == null) throw new ArgumentNullException("parent");
            if (mutation == null) throw new ArgumentNullException("mutation");
            _mutation = mutation;
        }

        protected override string NodeDescription
        {
            get { return _mutation.Description; }
        }

        /// <summary>
        /// Initialize a T starting from the parent's arrangement and applying this's mutation
        /// </summary>
        /// <returns></returns>
        public override T Arrange()
        {
            var startFrom = Parent.Arrange();
            _mutation.Apply(startFrom);
            return startFrom;
        }
    }
}