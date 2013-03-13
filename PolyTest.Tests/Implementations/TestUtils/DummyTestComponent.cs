using System;
using System.Collections.Generic;
using System.Linq;

namespace PolyTest.Tests.Implementations.TestUtils
{
    public class DummyTestComponent<T> : ITestComponent<T>
    {
        public string Description { get; set; }
        public T Arrange()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ITestCase<T>> Enumerate()
        {
            return Enumerable.Range(0, 3).Select(i => new DummyTestCase<T>(String.Format("Dummy Test case #{0}", i)));
        }

        public IEnumerable<ITestComponent<T>> Children { get; private set; }
    }
}