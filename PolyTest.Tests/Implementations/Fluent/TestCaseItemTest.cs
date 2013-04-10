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


        #region Test Helper Methods
        private static TestCaseItem<ClassToTest> MakeSut(int index = 3, ITestCase<ClassToTest> testCase = null)
        {

            testCase = testCase ?? new DummyTestCase<ClassToTest>("dumm test case");
            return new TestCaseItem<ClassToTest>(index, testCase);
        }

        #endregion
    }
}
