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
            var instanceToMutate = new ClassToTest(initialValue:5);
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

        [Fact]
        public void Root_returns_a_TestRoot()
        {
            // Arrange
            var sut = MakeSut();

            // Act
            var actual = sut.Root("whatever", () => new ClassToTest(2));

            // Assert
            Assert.IsType<TestRoot<ClassToTest>>(actual);
        }

        [Fact]
        public void Root_returns_TestRoot_with_Description()
        {
            // Arrange
            var expectedDescription = "some description";
            var sut = MakeSut();

            // Act
            var actual = sut.Root(expectedDescription, () => new ClassToTest(4));

            // Assert
            Assert.Equal(expectedDescription, actual.Description);
        }

        [Fact]
        public void Root_returns_TestRoot_with_Arrange_calling_setupFunction()
        {
            // Arrange
            var arrangeReturn = new ClassToTest(3);
            Func<ClassToTest> setUpFunction = () => arrangeReturn;
            var sut = MakeSut();
            var root = sut.Root("whatever", setUpFunction);

            // Act
            var actual = root.Arrange();

            // Assert
            Assert.Same(arrangeReturn, actual);
        }

        #endregion

        #region Leaf<>()

        [Fact]
        public void Leaf_returns_a_TestLeaf()
        {
            // Arrange
            var sut = MakeSut();

            // Act
            var actual = sut.Leaf(new DummyTestComposite<ClassToTest>(), new DummyMutation<ClassToTest>());

            // Assert
            Assert.IsType<TestLeaf<ClassToTest>>(actual);
        }

        [Fact]
        public void Leaf_with_ConditionToTest_returns_a_TestLeaf_with_Description_from_parent_AND_its_own()
        {
            // Arrange
            var parentDescription = "PARENT DESCRIPTION";
            var mutationDescription = "MUTATION DESCRIPTION";
            var sut = MakeSut();
            
            // Act
            var actual = sut.Leaf(new DummyTestComposite<ClassToTest>(parentDescription), new DummyMutation<ClassToTest>(mutationDescription));

            // Assert
            Assert.Equal(parentDescription + " AND " + mutationDescription, actual.Description);
        }

        [Fact]
        public void Leaf_with_ConditionToTest_returns_a_TestLeaf_with_Arrange_from_parent_Arrange_AND_its_own()
        {
            // Arrange
            var sut = MakeSut();
            var arrangeReturn = new ClassToTest(5);
            var parent = new DummyTestComposite<ClassToTest>();
            parent.StubbedArrange = () => arrangeReturn;
            var mutationNewIntProperty = 666;
            var mutation = new DummyMutation<ClassToTest>();
            mutation.StubbedApply = c => c.IntProperty = mutationNewIntProperty;
            var leaf = sut.Leaf(parent, mutation);

            // Act
            var actual = leaf.Arrange();

            // Assert
            Assert.Same(arrangeReturn, actual);
            Assert.Equal(actual.IntProperty, mutationNewIntProperty);
        }

        #endregion



        #region Test Helper Methods

        PolyDefaultTestFactory MakeSut()
        {
            return new PolyDefaultTestFactory();
        }

        #endregion
    }
}
