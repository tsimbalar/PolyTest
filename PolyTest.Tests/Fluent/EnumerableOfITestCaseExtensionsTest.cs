using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolyTest.Fluent;
using PolyTest.Implementations.Fluent;
using PolyTest.Tests.TestUtils;
using Xunit;

namespace PolyTest.Tests.Fluent
{
    public class EnumerableOfITestCaseExtensionsTest
    {

        [Fact]
        public void Test_calls_act_for_each_element_in_wrapped_Enumerate()
        {
            // Arrange
            var sut = MakeSut();
            var actCallCounter = 0;

            // Act
            var actual = sut.Test(act: c =>
                              {
                                  actCallCounter = actCallCounter + 1;
                                  return c;
                              },
                     assert: c => { }); // nothing to assert in this test

            // Assert
            Assert.Equal(sut.Count(), actCallCounter);
        }

        

        [Fact]
        public void TEst_calls_assert_for_each_element_in_wrapped_Enumerate()
        {
            // Arrange
            var sut = MakeSut();
            var assertCallCounter = 0;

            // Act
            var actual = sut.Test(
                    act: c => c,
                    assert: c => assertCallCounter++); // nothing to assert in this test

            // Assert
            Assert.Equal(sut.Count(), assertCallCounter);
        }

        [Fact]
        public void Test_returns_a_TestExecutionReport()
        {
            // Arrange
            var sut = MakeSut();

            // Act
            var actual = sut.Test(
                act: c => c,
                assert: c => { }
                );

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<TestExecutionReport<ClassToTest>>(actual);
        }

        [Fact]
        public void Test_returns_one_result_for_each_testCase()
        {
            // Arrange
            var sut = MakeSut();

            // Act
            var actual = sut.Test(
                act: c => c,
                assert: c => { }
                );

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(sut.Count(), actual.Count);
            Assert.Equal(sut.Select(tc => tc.Description), actual.All.Select(tr => tr.TestCase.Description));
        }


        #region Test Helper Methods
        private IEnumerable<ITestCase<ClassToTest>> MakeSut(int count = 5)
        {
            var result = new List<DummyTestCase<ClassToTest>>();
            for (int i = 0; i < count; i++)
            {
                var indexForDisplay = i + 1;
                var testCase = MakeTestCase(indexForDisplay);
                result.Add(testCase);
            }
            return result;
        }

        private static DummyTestCase<ClassToTest> MakeTestCase(int identifier)
        {
            var testCase = new DummyTestCase<ClassToTest>(String.Format("test case {0}", identifier));
            testCase.StubbedArrange = () => new ClassToTest(identifier);
            return testCase;
        }

        #endregion
    }
}
