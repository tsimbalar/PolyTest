using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolyTest.Implementations;
using PolyTest.Tests.Implementations.TestUtils;
using Xunit;

namespace PolyTest.Tests.Implementations
{
    public class TestRootTest
    {
        [Fact]
        public void Ctor_with_description_null_throws_ArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new TestRoot<ClassToTest>(null, () => new ClassToTest(1))
            );
        }

        [Fact]
        public void Ctor_assigns_Description()
        {
            // Arrange
            var someString = "some string";

            // Act
            var sut = MakeSut(description: someString);

            // Assert
            Assert.Equal(someString, sut.Description);
        }

        [Fact]
        public void Ctor_with_setUpFunction_null_throws_ArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new TestRoot<ClassToTest>("some text", null)
                );
        }

        [Fact]
        public void Ctor_assigns_IncludeSelfInEnumeration_false()
        {
            // Arrange

            // Act
            var sut = MakeSut();

            // Assert
            Assert.False(sut.IncludeSelfInEnumeration);
        }

        [Fact]
        public void Ctor_assigns_Children_empty()
        {
            // Arrange

            // Act
            var sut = MakeSut();

            // Assert
            Assert.Empty(sut.Children);
        }

        [Fact]
        public void Enumerate_after_construction_returns_empty_Enumerable()
        {
            // Arrange
            var sut = MakeSut();

            // Act
            var actual = sut.Enumerate();

            // Assert
            Assert.Equal(Enumerable.Empty<ITestCase<ClassToTest>>(), actual);
        }

        [Fact]
        public void Arrange_returns_result_of_call_setupFunction()
        {
            // Arrange
            var valueToReturn = new ClassToTest(666);
            Func<ClassToTest> setupFunction = () => valueToReturn;
            var sut = MakeSut(setup:setupFunction);

            // Act
            var actual = sut.Arrange();

            // Assert
            Assert.Same(valueToReturn, actual);

        }

        [Fact]
        public void Add_adds_item_to_Children()
        {
            // Arrange
            var child = new DummyTestComponent<ClassToTest>();
            var sut = MakeSut();

            // Act
            sut.Add(child);

            // Assert
            Assert.True(sut.Children.Contains(child));
        }

        [Fact]
        public void Enumerate_enumerates_over_children_and_children_Enumerate()
        {
            // Arrange
            var sut = MakeSut();
            var child = new DummyTestComponent<ClassToTest>();
            sut.Add(child);

            // Act
            var actual = sut.Enumerate();

            // Assert
            Assert.Equal(child.Enumerate(), actual);
        }

        #region Test Helper Methods

        private static TestRoot<ClassToTest> MakeSut(string description = "any description", Func<ClassToTest> setup = null )
        {
            setup = setup ?? ( ()=> new ClassToTest(1));

            return new TestRoot<ClassToTest>(description, setup);
        } 

        #endregion
    }
}
