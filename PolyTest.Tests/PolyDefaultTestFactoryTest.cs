using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolyTest.Implementations;
using PolyTest.Tests.TestUtils;
using Xunit;

namespace PolyTest.Tests
{
    public class PolyDefaultTestFactoryTest
    {
        #region Mutation<>()

        [Fact]
        public void Mutation_returns_a_Mutation()
        {
            // Arrange
            Action<ClassToTest> mutationAction = (c) => {};
            var sut = MakeSut();

            // Act
            var actual = sut.Mutation("whatever", mutationAction);

            // Assert
            Assert.IsType<Mutation<ClassToTest>>(actual);
        }

        [Fact]
        public void Mutation_returns_a_Mutation_with_Description()
        {
            // Arrange
            string expectedDescription = "bli blo blah";
            Action<ClassToTest> mutationAction = (c) => {};
            var sut = MakeSut();

            // Act
            var actual = sut.Mutation(expectedDescription, mutationAction);

            // Assert
            Assert.Equal(expectedDescription, actual.Description);
        }

        [Fact]
        public void Mutation_returns_a_Mutation_with_Apply_calling_mutationAction()
        {
            // Arrange
            ClassToTest instanceToMutate = new ClassToTest(initialValue:5);
            ClassToTest mutatedInstance = null;
            Action<ClassToTest> mutationAction = (c) => { mutatedInstance = c; };
            var sut = MakeSut();
            var actual = sut.Mutation("whatever", mutationAction);

            // Act
            actual.Apply(instanceToMutate);

            // Assert
            Assert.Same(instanceToMutate, mutatedInstance);
        }
        #endregion

        #region Root<>()



        #endregion

        #region Test Helper Methods

        PolyDefaultTestFactory MakeSut()
        {
            return new PolyDefaultTestFactory();
        }

        #endregion
    }
}
