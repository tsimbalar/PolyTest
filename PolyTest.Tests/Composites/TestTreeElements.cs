using System;
using System.Collections.Generic;

namespace PolyTest.Tests.Composites
{

    /// <summary>
    /// The component that is the common part of every element in the tree
    /// </summary>
    /// <typeparam name="T">the type that the test cases initialize</typeparam>
    public interface ITestComponent<T> : ITestCase<T>
    {
        IEnumerable<ITestCase<T>> Enumerate();
    }

    /// <summary>
    /// The composite in the tree, that is, a component with children
    /// </summary>
    public interface ITestComposite<T> : ITestComponent<T>
    {
        void Add(ITestComponent<T> child);

        /// <summary>
        /// Indicates whether the Enumerate() method returns an element for this node, or if it should just return its children
        /// </summary>
        bool IncludeSelfInEnumeration { get; set; }
    }

    /// <summary>
    /// Default implementation of ITestComponent
    /// Generates Description based on parent description + this node's description
    /// Provides access to Parent for subclasses
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal abstract class TestComponentBase<T> : ITestComponent<T>
    {
        private readonly ITestComposite<T> _parent;

        /// <summary>
        /// Create a TestComponent
        /// </summary>
        /// <param name="parent">the parent of this node (can be null)</param>
        protected TestComponentBase(ITestComposite<T> parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// The Description for this element. Based on the parent's description (if there is a parent) and this node's description
        /// </summary>
        public virtual string Description
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

        /// <summary>
        /// The parent of this component (can be null)
        /// </summary>
        protected ITestComposite<T> Parent { get { return _parent; } }

        /// <summary>
        /// Enumerate over the test cases of this component 
        /// </summary>
        public virtual IEnumerable<ITestCase<T>> Enumerate()
        {
            yield return this;
        }

        protected abstract string NodeDescription { get; }

        /// <summary>
        /// TestCase setup for this component
        /// </summary>
        /// <returns></returns>
        public abstract T Arrange();
    }

    /// <summary>
    /// Default implementation of the ITestComposite&lt;T&gt;
    /// Takes care of adding children and enumerating over its children
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal abstract class TestCompositeBase<T> : TestComponentBase<T>, ITestComposite<T>
    {
        private readonly List<ITestComponent<T>> _children;

        /// <summary>
        /// Creates a composite
        /// </summary>
        /// <param name="parent">the parent of this node</param>
        /// <param name="includeInEnumeration">when enumerating/walking over the tree, should this node be included, or should it just enumerate over its children ?</param>
        protected TestCompositeBase(ITestComposite<T> parent,bool includeInEnumeration)
            : base(parent)
        {
            IncludeSelfInEnumeration = includeInEnumeration;
            _children = new List<ITestComponent<T>>();
        }

        /// <summary>
        /// Indicates whether the Enumerate() method returns an element for this node, or if it should just return its children
        /// </summary>
        public bool IncludeSelfInEnumeration { get; set; }

        public void Add(ITestComponent<T> child)
        {
            _children.Add(child);
        }

        /// <summary>
        /// Enumerate over all the test cases, including the children and itself if IncludeSelfInEnumeration is true
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<ITestCase<T>> Enumerate()
        {
            if (IncludeSelfInEnumeration)
            {
                yield return this;
            }
            foreach (var testComponent in _children)
            {
                foreach (var testCase in testComponent.Enumerate())
                {
                    yield return testCase;
                }
            }
        }
    }

    /// <summary>
    /// The root of the tree of testcases
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class TestRoot<T> : TestCompositeBase<T>
    {
        private readonly string _description;
        private readonly Func<T> _setup;

        /// <summary>
        /// Creates a root for the tree of testcases
        /// </summary>
        /// <param name="description">the descritpion of the initial condition</param>
        /// <param name="setup">how to obtain the initial element that is used as a base for all tests</param>
        public TestRoot(string description, Func<T> setup)
            : base(null, false /* do not enumerate over self by default*/)
        {
            if (description == null) throw new ArgumentNullException("description");
            if (setup == null) throw new ArgumentNullException("setup");
            _description = description;
            _setup = setup;
        }

        protected override string NodeDescription
        {
            get { return _description; }
        }

        /// <summary>
        /// Get a T in its initial state
        /// </summary>
        /// <returns></returns>
        public override T Arrange()
        {
            return _setup();
        }
    }

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
        /// <param name="includeInEnumeration">should this node be considered a test case, or is it just a way of grouping the children ?</param>
        public TestComposite(ITestComposite<T> parent, IMutation<T> mutation , bool includeInEnumeration)
            : base(parent, includeInEnumeration)
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

    /// <summary>
    /// The bottom element of the tree, a test case with no children
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class TestLeaf<T> : TestComponentBase<T>
    {
        private readonly IMutation<T> _mutation;

        public TestLeaf(ITestComposite<T> parent, IMutation<T> mutation )
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

        public override T Arrange()
        {
            var startFrom = Parent.Arrange();
            _mutation.Apply(startFrom);
            return startFrom;
        }
    }
}
