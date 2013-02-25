using System.Collections.Generic;

namespace PolyTest.Implementations
{
    internal abstract class TestTreeBase<T> : ITestComponent<T>
    {
        private readonly ITestComposite<T> _parent;
        private readonly IList<ITestComponent<T>> _children ;

        /// <summary>
        /// Create a TestComponent
        /// </summary>
        /// <param name="parent">the parent of this node (can be null)</param>
        protected TestTreeBase(ITestComposite<T> parent)
        {
            _parent = parent;
            _children = new List<ITestComponent<T>>();
        }
        /// <summary>
        /// The parent of this component (can be null)
        /// </summary>
        protected ITestComposite<T> Parent { get { return _parent; } }


        /// <summary>
        /// The Description for this element. Based on the parent's description (if there is a parent) and this node's description
        /// </summary>
        public string Description
        {
            get
            {
                if (Parent == null)
                {
                    return this.NodeDescription;
                }
                else
                {
                    return Parent.Description + " AND " + this.NodeDescription;
                }
            }
        }

        protected abstract string NodeDescription { get; }

        public IEnumerable<ITestComponent<T>> Children { get { return _children; } }

        protected void Add(ITestComponent<T> child)
        {
            _children.Add(child);
        }

        /// <summary>
        /// Indicates whether the Enumerate() method returns an element for this node, or if it should just return its children
        /// </summary>
        public abstract bool IncludeSelfInEnumeration { get; }

        /// <summary>
        /// Enumerate over all the test cases, including the children and itself if IncludeSelfInEnumeration is true
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ITestCase<T>> Enumerate()
        {
            if (IncludeSelfInEnumeration)
            {
                yield return this;
            }
            foreach (var testComponent in Children)
            {
                foreach (var testCase in testComponent.Enumerate())
                {
                    yield return testCase;
                }
            }
        }

        /// <summary>
        /// TestCase setup for this component
        /// </summary>
        /// <returns></returns>
        public abstract T Arrange();

    }
}