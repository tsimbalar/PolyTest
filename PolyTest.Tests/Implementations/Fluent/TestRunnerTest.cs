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
    public class TestRunnerTest
    {

        [Fact]
        public void Run_with_testCase_null_throws_ArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                             TestRunner.Run(2, (ITestCase<ClassToTest>)null, c => c, c => { })
                );
        }

        [Fact]
        public void Run_returns_TestCase()
        {
            // Arrange
            var expectedIndex = 3;
            var expectedDescription = "some description";
            var testCase = new DummyTestCase<ClassToTest>(expectedDescription);
            testCase.StubbedArrange = () => new ClassToTest();
            

            // Act
            var actual = TestRunner.Run(expectedIndex, testCase,
                act: c => c,
                assert: a => {/* nothing to do ...*/}
                );

            // Assert
            // Test Case information
            Assert.Equal(expectedDescription, actual.TestCase.Description);
            Assert.Equal(expectedIndex, actual.TestCase.Index);
        }

        [Fact]
        public void Run_with_error_during_Arrange_returns_Failed_result()
        {
            // Arrange
            var testCase = new DummyTestCase<ClassToTest>("whatever");

            var arrangeException = new DummyException("it failed");
            testCase.StubbedArrange = () => { throw arrangeException; };


            // Act
            var actual = TestRunner.Run(2, testCase,
                act: c => c,
                assert: a => {/* do nothing */}
                );

            // Assert
            Assert.NotNull(actual);
            // Result information
            Assert.False(actual.IsSuccess, "should not be a Success result");
            Assert.False(actual.HasResult, "Should not have a result, because act did fail");
            Assert.Null(actual.Result);
            Assert.Equal("KO", actual.ResultMessage.Split('\n')[0]);
            Assert.Contains(arrangeException.GetType().Name + " thrown during Arrange", actual.ResultMessage);
            Assert.Contains(arrangeException.ToString(), actual.ResultMessage);
        }

        [Fact]
        public void Run_with_error_during_act_returns_Failed_result()
        {
            // Arrange
            var testCaseArrangeResult = new ClassToTest();
            var testCase = new DummyTestCase<ClassToTest>("whatever");
            testCase.StubbedArrange = () => testCaseArrangeResult;
            var actException = new DummyException("it failed");

            // Act
            var actual = TestRunner.Run(2, testCase,
                act: c =>
                {
                    throw actException;
#pragma warning disable 162 // "Unreachable code detected" - need it for the type inference to work
                    return c;
#pragma warning restore 162
                },
                assert: a => {/* do nothing*/}
                );

            // Assert
            Assert.NotNull(actual);
            // Result information
            Assert.False(actual.IsSuccess, "should not be a Success result");
            Assert.False(actual.HasResult, "Should not have a result, because act did fail");
            Assert.Null(actual.Result);
            Assert.Equal("KO", actual.ResultMessage.Split('\n')[0]);
            Assert.Contains(actException.GetType().Name + " thrown during Act", actual.ResultMessage);
            Assert.Contains(actException.ToString(), actual.ResultMessage);
        }

        [Fact]
        public void Run_with_assertion_failed_returns_Failed_result()
        {
            // Arrange
            var testCaseArrangeResult = new ClassToTest();
            var testCase = new DummyTestCase<ClassToTest>("whatever");
            testCase.StubbedArrange = () => testCaseArrangeResult;
            var assertionException = new FalseException("The Assertion has failed - some user message");

            // Act
            var actual = TestRunner.Run(2, testCase,
                act: c => c,
                assert: a =>
                {
                    throw assertionException;
                }
                );

            // Assert
            Assert.NotNull(actual);
            // Result information
            Assert.False(actual.IsSuccess, "should not be a Success result");
            Assert.True(actual.HasResult, "Should have a result, because act did not fail");
            Assert.Equal(testCaseArrangeResult, actual.Result);
            Assert.Equal("returned " + testCaseArrangeResult.ToString() + " - KO", actual.ResultMessage.Split('\n')[0]);
            Assert.Contains(assertionException.GetType().Name + " thrown during Assert", actual.ResultMessage);
            Assert.Contains(assertionException.ToString(), actual.ResultMessage);
        }

        [Fact]
        public void Run_with_assertion_success_returns_Success_result()
        {
            // Arrange
            var testCaseArrangeResult = new ClassToTest();
            var testCase = new DummyTestCase<ClassToTest>("whatever");
            testCase.StubbedArrange = () => testCaseArrangeResult;

            // Act
            var actual = TestRunner.Run(2, testCase,
                act: c => c,
                assert: a => {/* nothing to do ...*/}
                );

            // Assert
            Assert.NotNull(actual);
            // Result information
            Assert.True(actual.IsSuccess, "should be a Success result");
            Assert.True(actual.HasResult, "Should have a result, because act did not fail");
            Assert.Equal(testCaseArrangeResult, actual.Result);
            Assert.Equal("returned " + testCaseArrangeResult.ToString() + " - OK", actual.ResultMessage.Split('\n')[0]);

        }
    }
}
