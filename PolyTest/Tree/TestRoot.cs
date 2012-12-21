using System;

namespace PolyTest.Tree
{
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
}