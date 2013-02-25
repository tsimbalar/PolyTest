namespace PolyTest.Implementations
{
    /// <summary>
    /// Default implementation of the ITestComposite&lt;T&gt;
    /// Takes care of adding children and enumerating over its children
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal abstract class TestCompositeBase<T> : TestTreeBase<T>, ITestComposite<T>
    {
        private bool _includeSelfInEnumeration;

        /// <summary>
        /// Creates a composite
        /// </summary>
        /// <param name="parent">the parent of this node</param>
        protected TestCompositeBase(ITestComposite<T> parent)
            : this(parent, true)
        {
        }

        /// <summary>
        /// Creates a composite
        /// </summary>
        /// <param name="parent">the parent of this node</param>
        /// <param name="includeInEnumeration">when enumerating/walking over the tree, should this node be included, or should it just enumerate over its children ?</param>
        protected TestCompositeBase(ITestComposite<T> parent, bool includeInEnumeration)
            : base(parent)
        {
            _includeSelfInEnumeration = includeInEnumeration;
        }

        public new void Add(ITestComponent<T> child)
        {
            base.Add(child);
        }

        /// <summary>
        /// Indicates whether the Enumerate() method returns an element for this node, or if it should just return its children
        /// </summary>
        public override bool IncludeSelfInEnumeration
        {
            get { return _includeSelfInEnumeration; }
        }

        bool ITestComposite<T>.IncludeSelfInEnumeration
        {
            get { return _includeSelfInEnumeration; }
            set { _includeSelfInEnumeration = value; }
        }
    }
}