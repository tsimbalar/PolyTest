using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolyTest.Implementations;
using PolyTest.Tests.Implementations.TestUtils;
using Xunit;

namespace PolyTest.Tests.Implementations
{
    public class MutationTest
    {
        [Fact]
        public void Ctor_with_mutationDescription_null_throws_ArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new Mutation<ClassToTest>(null, d => d.IntProperty = 2)
            );
        }

        [Fact]
        public void Ctor_with_mutationToApply_null_throws_ArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new Mutation<ClassToTest>("someString", null)
            );
        }

        [Fact]
        public void Ctor_assigns_property_Description()
        {
            // Arrange
            var someString = "any string";
            
            // Act
            var sut = new Mutation<ClassToTest>(someString, d => d.IntProperty = 2);

            // Assert
            Assert.Equal(someString, sut.Description);
        }

        [Fact]
        public void Apply_calls_mutationToApply_on_argument()
        {
            // Arrange
            var initialValue = 4;
            var initial = new ClassToTest(initialValue);
            var sut = new Mutation<ClassToTest>("some string", d => d.IntProperty += 2);

            // Act
           sut.Apply(initial);

            // Assert
            Assert.Equal(initialValue + 2, initial.IntProperty);
        }

    }
}
