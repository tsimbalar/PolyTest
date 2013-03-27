using System;
using System.Collections.Generic;

namespace PolyTest.Tests.TestUtils
{
    public class DummyTestComposite<T> : ITestComposite<T>
    {
        private readonly string _description;
        private readonly List<ITestComponent<T>> _children;
        private readonly List<ITestCase<T>> _testCases; 

        public DummyTestComposite(string description= "dummy test composite description")
        {
            _description = description;
            _children = new List<ITestComponent<T>>();
            _testCases = new List<ITestCase<T>>();
        }

        public string Description { get { return _description; } }
        public T Arrange()
        {
            return StubbedArrange();
        }


        /// <summary>
        /// In this version, ENumerate will enumerate over this list ...
        /// </summary>
        public List<ITestCase<T>> TestCases { get { return _testCases; } }
        public IEnumerable<ITestCase<T>> Enumerate()
        {
            return TestCases;
        }


        public IEnumerable<ITestComponent<T>> Children { get { return _children; } }

        public void Add(ITestComponent<T> child)
        {
            _children.Add(child);
        }

        public bool IncludeSelfInEnumeration { get; set; }

        /// <summary>
        /// Called when Arrange is called
        /// </summary>
        public Func<T> StubbedArrange { get; set; }
    }
}