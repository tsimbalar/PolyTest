using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolyTest.Implementations;
using PolyTest.Implementations.Fluent;
using PolyTest.Tests.TestUtils;
using Xunit;

namespace PolyTest.Tests.Implementations.Fluent
{
    public class TestRootFluentWrapperTest
    {
        [Fact]
        public void Ctor_with_wrapped_null_throws_ArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                    new TestRootFluentWrapper<ClassToTest>(null)
                );
        }

        [Fact]
        public void Ctor_assigns_Wrapped()
        {
            // Arrange
            var expectedWrapped = new DummyTestComposite<ClassToTest>();
            var sut = MakeSut(expectedWrapped);

            // Act
            var actual = sut.Wrapped;

            // Assert
            Assert.Same(expectedWrapped, actual);
        }

        [Fact]
        public void IncludeSelf_sets_Wrapped_IncludeSelfInEnumeration_true()
        {
            // Arrange
            var wrapped = new DummyTestComposite<ClassToTest>();
            wrapped.IncludeSelfInEnumeration = false;
            var sut = MakeSut(wrapped);

            // Act
            sut.IncludeSelf("no reason");

            // Assert
            Assert.True(wrapped.IncludeSelfInEnumeration);
        }

        [Fact]
        public void Consider_returns_self()
        {
            // Arrange
            var sut = MakeSut();
            var mutationToAdd = new DummyMutation<ClassToTest>();
            
            // Act
            var actual = sut.Consider(mutationToAdd);

            // Assert
            Assert.Same(sut, actual);
        }

        [Fact]
        public void Consider_with_ConditionToTest_adds_TestComposite_to_wrapped()
        {
            // Arrange
            var wrapped = new DummyTestComposite<ClassToTest>();
            var sut = MakeSut(wrapped);
            var mutationToAdd = new DummyMutation<ClassToTest>();
            
            // Act
            sut.Consider(mutationToAdd);

            // Assert
            Assert.IsType<TestComposite<ClassToTest>>(wrapped.Children.Single());
            var actualChild = (TestComposite<ClassToTest>) wrapped.Children.Single();
            Assert.Equal(wrapped.Description + " AND " +  mutationToAdd.Description, actualChild.Description);
        }



        private TestRootFluentWrapper<ClassToTest> MakeSut(ITestComposite<ClassToTest> wrapped = null)
        {
            wrapped = wrapped ?? new DummyTestComposite<ClassToTest>();
            return new TestRootFluentWrapper<ClassToTest>(wrapped);
        }
    }
}
