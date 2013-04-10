using System;
using PolyTest.Fluent;

namespace PolyTest.Implementations.Fluent
{
    internal class TestCaseItem<T> : ITestCaseInformation, ITestCase<T>
    {
        private readonly int _index;
        private readonly ITestCase<T> _testCase;

        public TestCaseItem(int index, ITestCase<T> testCase)
        {
            if (testCase == null) throw new ArgumentNullException("testCase");
            _index = index;
            _testCase = testCase;
        }

        public string Description { get { return _testCase.Description; } }

        public int Index
        {
            get { return _index; }
        }

        public T Arrange()
        {
            return _testCase.Arrange();
        }

        public override string ToString()
        {
            return string.Format("#{0} - {1}", Index, Description);
        }
    }
}