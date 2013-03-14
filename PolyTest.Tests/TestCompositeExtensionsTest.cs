using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolyTest.Tests.TestUtils;
using Xunit;

namespace PolyTest.Tests
{
    public class TestCompositeExtensionsTest
    {
        #region Walk

        [Fact]
        public void Walk_calls_action_on_each_test_case_of_Component_Enumerate()
        {
            // Arrange
            var sut = new MockTestComponent();
            var testCases = Enumerable.Range(1, 5).Select(i => new DummyTestCase<ClassToTest>(String.Format("Test case #{0}", i))).ToList();
            sut.TestCases.AddRange(testCases);
            var loopedThroughTestCases = new List<ITestCase<ClassToTest>>();
            Action<ITestCase<ClassToTest>> action = (t) => loopedThroughTestCases.Add(t);

            // Act
            sut.Walk(action);

            // Assert
            Assert.Equal(testCases, loopedThroughTestCases);
        }

        #endregion

        private class MockTestComponent : ITestComponent<ClassToTest>
        {
            public List<ITestCase<ClassToTest>> TestCases { get; private set; }

            public MockTestComponent()
            {
                TestCases = new List<ITestCase<ClassToTest>>();
            }

            public string Description { get; private set; }
            public ClassToTest Arrange()
            {
                throw new NotImplementedException();
            }

            public IEnumerable<ITestCase<ClassToTest>> Enumerate()
            {
                return TestCases;
            }

            public IEnumerable<ITestComponent<ClassToTest>> Children { get; private set; }
        }

        private class DummyTestCase<T> : ITestCase<T>
        {
            private readonly string _description;

            public DummyTestCase(string description = "Dummy test case")
            {
                _description = description;
            }

            public string Description { get { return _description; } }
            public T Arrange()
            {
                throw new NotImplementedException();
            }
        }
    }
}
