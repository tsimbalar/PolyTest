using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolyTest.Implementations.Fluent;
using PolyTest.Tests.TestUtils;
using Xunit;
using Xunit.Sdk;

namespace PolyTest.Tests.Implementations.Fluent
{
    public class TestCaseItemTest
    {
        [Fact]
        public void Ctor_with_TestCase_null_throws_ArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                             new TestCaseItem<ClassToTest>(4, null)
                );
        }

        [Fact]
        public void Ctor_assigns_index()
        {
            // Arrange
            int expectedIndex = 235;
            // Act
            var sut = MakeSut(expectedIndex);

            // Assert
            Assert.Equal(expectedIndex, sut.Index);
        }

        [Fact]
        public void Description_returns_Description_from_TestCase()
        {
            // Arrange
            string expectedDescription = "some descripton";
            var testCase = new DummyTestCase<ClassToTest>(expectedDescription);
            // Act
            var sut = MakeSut(testCase: testCase);

            // Assert
            Assert.Equal(expectedDescription, sut.Description);
        }

        [Fact]
        public void Arrange_with_ConditionToTest_calls_TestCase_Arrange()
        {
            // Arrange
            var toReturn = new ClassToTest(666);
            var testCase = new DummyTestCase<ClassToTest>("whatever");
            testCase.StubbedArrange = () => toReturn;
            var sut = MakeSut(testCase: testCase);

            // Act
            var actual = sut.Arrange();

            // Assert
            Assert.Same(toReturn, actual);
        }

        [Fact]
        public void ToString_returns_string_with_Index_and_Description()
        {
            // Arrange
            var index = 469;
            var description = "some description bla bla";
            var expectedToString = String.Format("#{0} - {1}", index, description);
            var sut = MakeSut(index, new DummyTestCase<ClassToTest>(description));

            // Act
            var actual = sut.ToString();

            // Assert
            Assert.Equal(expectedToString, actual);
        }


        [Fact]
        public void Test_with_error_during_Arrange_returns_Failed_result()
        {
            // Arrange
            var testCase = new DummyTestCase<ClassToTest>("whatever");

            var arrangeException = new DummyException("it failed");
            testCase.StubbedArrange = () => { throw arrangeException; };
            var sut = MakeSut(testCase: testCase);



            // Act
            var actual = sut.Test(
                act: c => c,
                assert: a => {/* do nothing */}
                );

            // Assert
            Assert.NotNull(actual);
            // Test Case information
            Assert.Equal(testCase.Description, actual.TestCase.Description);
            Assert.Equal(sut.Index, actual.TestCase.Index);
            // Result information
            Assert.False(actual.IsSuccess, "should not be a Success result");
            Assert.False(actual.HasResult, "Should not have a result, because act did fail");
            Assert.Null(actual.Result);
            Assert.Equal("KO", actual.ResultMessage.Split('\n')[0]);
            Assert.Contains(arrangeException.GetType().Name + " thrown during Arrange", actual.ResultMessage);
            Assert.Contains(arrangeException.ToString(), actual.ResultMessage);
        }

        [Fact]
        public void Test_with_error_during_act_returns_Failed_result()
        {
            // Arrange
            var testCaseArrangeResult = new ClassToTest(45600);
            var testCase = new DummyTestCase<ClassToTest>("whatever");
            testCase.StubbedArrange = () => testCaseArrangeResult;
            var sut = MakeSut(testCase: testCase);

            var actException = new DummyException("it failed");

            // Act
            var actual = sut.Test(
                act: c =>
                         {
                             throw actException;
                             return c;
                         },
                assert: a => {/* do nothing*/}
                );

            // Assert
            Assert.NotNull(actual);
            // Test Case information
            Assert.Equal(testCase.Description, actual.TestCase.Description);
            Assert.Equal(sut.Index, actual.TestCase.Index);
            // Result information
            Assert.False(actual.IsSuccess, "should not be a Success result");
            Assert.False(actual.HasResult, "Should not have a result, because act did fail");
            Assert.Null(actual.Result);
            Assert.Equal("KO", actual.ResultMessage.Split('\n')[0]);
            Assert.Contains(actException.GetType().Name + " thrown during Act", actual.ResultMessage);
            Assert.Contains(actException.ToString(), actual.ResultMessage);
        }

        [Fact]
        public void Test_with_assertion_failed_returns_Failed_result()
        {
            // Arrange
            var testCaseArrangeResult = new ClassToTest(45600);
            var testCase = new DummyTestCase<ClassToTest>("whatever");
            testCase.StubbedArrange = () => testCaseArrangeResult;
            var sut = MakeSut(testCase: testCase);

            var assertionException = new FalseException("The Assertion has failed - some user message");

            // Act
            var actual = sut.Test(
                act: c => c,
                assert: a =>
                            {
                                throw assertionException;
                            }
                );

            // Assert
            Assert.NotNull(actual);
            // Test Case information
            Assert.Equal(testCase.Description, actual.TestCase.Description);
            Assert.Equal(sut.Index, actual.TestCase.Index);
            // Result information
            Assert.False(actual.IsSuccess, "should not be a Success result");
            Assert.True(actual.HasResult, "Should have a result, because act did not fail");
            Assert.Equal(testCaseArrangeResult, actual.Result);
            Assert.Equal("returned " + testCaseArrangeResult.ToString() + " - KO", actual.ResultMessage.Split('\n')[0]);
            Assert.Contains(assertionException.GetType().Name + " thrown during Assert", actual.ResultMessage);
            Assert.Contains(assertionException.ToString(), actual.ResultMessage);
        }

        [Fact]
        public void Test_with_assertion_success_returns_Success_result()
        {
            // Arrange
            var testCaseArrangeResult = new ClassToTest(45600);
            var testCase = new DummyTestCase<ClassToTest>("whatever");
            testCase.StubbedArrange = () => testCaseArrangeResult;
            var sut = MakeSut(testCase: testCase);

            // Act
            var actual = sut.Test(
                act: c => c,
                assert: a => {/* nothing to do ...*/}
                );

            // Assert
            Assert.NotNull(actual);
            // Test Case information
            Assert.Equal(testCase.Description, actual.TestCase.Description);
            Assert.Equal(sut.Index, actual.TestCase.Index);
            // Result information
            Assert.True(actual.IsSuccess, "should be a Success result");
            Assert.True(actual.HasResult, "Should have a result, because act did not fail");
            Assert.Equal(testCaseArrangeResult, actual.Result);
            Assert.Equal("returned " + testCaseArrangeResult.ToString() + " - OK", actual.ResultMessage.Split('\n')[0]);

        }


        #region Test Helper Methods
        private static TestCaseItem<ClassToTest> MakeSut(int index = 3, ITestCase<ClassToTest> testCase = null)
        {

            testCase = testCase ?? new DummyTestCase<ClassToTest>("dumm test case");
            return new TestCaseItem<ClassToTest>(index, testCase);
        }

        #endregion
    }
}
