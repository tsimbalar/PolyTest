using System;
using System.Collections.Generic;

namespace PolyTest.Tests.TestUtils
{
    public class DummyTestComposite<T> : ITestComposite<T>
    {
        private readonly string _description;

        public DummyTestComposite(string description= "dummy test composite description")
        {
            _description = description;
        }

        public string Description { get { return _description; } }
        public T Arrange()
        {
            return StubbedArrange();
        }

        public IEnumerable<ITestCase<T>> Enumerate()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ITestComponent<T>> Children { get; private set; }
        public void Add(ITestComponent<T> child)
        {
            throw new NotImplementedException();
        }

        public bool IncludeSelfInEnumeration { get; set; }

        /// <summary>
        /// Called when Arrange is called
        /// </summary>
        public Func<T> StubbedArrange { get; set; }
    }
}