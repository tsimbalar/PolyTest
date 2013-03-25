using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolyTest.Implementations;
using PolyTest.Tests.TestUtils;
using Xunit;

namespace PolyTest.Tests.Implementations
{
    public class TestLeafTest
    {
        [Fact]
        public void Ctor_with_parent_null_throws_ArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                             new TestLeaf<ClassToTest>(null, new DummyMutation<ClassToTest>("the mutation"))
                );
        }

        [Fact]
        public void Ctor_with_mutation_null_throws_ArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                             new TestLeaf<ClassToTest>(new DummyTestComposite<ClassToTest>("parent"), null)
                );
        }

        [Fact]
        public void Ctor_assigns_IncludeSelfInEnumeration_true()
        {
            // Arrange

            // Act
            var sut = MakeSut();

            // Assert
            Assert.True(sut.IncludeSelfInEnumeration);
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
        public void Description_returns_Parent_descriptions_AND_mutation_Description()
        {
            // Arrange
            var mutationDescription = "la super description";
            var theMutation = new DummyMutation<ClassToTest>(mutationDescription);
            var parentDescription = "parent description";
            var theParent = new DummyTestComposite<ClassToTest>(parentDescription);
            var sut = MakeSut(theParent, theMutation);
            var expectedDescription = parentDescription + " AND " + mutationDescription;

            // Act
            var actual = sut.Description;

            // Assert
            Assert.Equal(expectedDescription, actual);
        }

        [Fact]
        public void Arrange_returns_result_of_applying_mutation_to_parent_Arrange()
        {
            // Arrange
            var parentArrangement = new ClassToTest(4);
            var theParent = new DummyTestComposite<ClassToTest>();
            theParent.StubbedArrange = () => parentArrangement;
            var theMutation = new DummyMutation<ClassToTest>();
            ClassToTest itemPassedToMutationApply = null;
            theMutation.StubbedApply = d => itemPassedToMutationApply = d;
            var sut = MakeSut(theParent, theMutation);
            
            // Act
            var actual = sut.Arrange();

            // Assert
            Assert.Same(parentArrangement, actual);
            Assert.Same(parentArrangement, itemPassedToMutationApply);
        }


        [Fact]
        public void Enumerate_returns_only_itself()
        {
            // Arrange
            var sut = MakeSut();

            // Act
            var actual = sut.Enumerate();

            // Assert
            Assert.Equal(Enumerable.Repeat(sut,1), actual);
        }

        #region Test Helper Methods

        private static TestLeaf<ClassToTest> MakeSut(ITestComposite<ClassToTest> parent = null, IMutation<ClassToTest> mutation = null)
        {
            parent = parent ?? new DummyTestComposite<ClassToTest>();
            mutation = mutation ?? new DummyMutation<ClassToTest>();

            return new TestLeaf<ClassToTest>(parent, mutation);
        }

        #endregion
    }
}
