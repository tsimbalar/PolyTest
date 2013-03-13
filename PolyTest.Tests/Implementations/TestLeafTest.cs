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
                             new TestLeaf<DummyItem>(null, new DummyMutation<DummyItem>("the mutation"))
                );
        }

        [Fact]
        public void Ctor_with_mutation_null_throws_ArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                             new TestLeaf<DummyItem>(new DummyTestComposite<DummyItem>("parent"), null)
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
            var theMutation = new DummyMutation<DummyItem>(mutationDescription);
            var parentDescription = "parent description";
            var theParent = new DummyTestComposite<DummyItem>(parentDescription);
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
            var parentArrangement = new DummyItem(4, true);
            var theParent = new DummyTestComposite<DummyItem>();
            theParent.StubbedArrange = () => parentArrangement;
            var theMutation = new DummyMutation<DummyItem>();
            DummyItem itemPassedToMutationApply = null;
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

        private TestLeaf<DummyItem> MakeSut(ITestComposite<DummyItem> parent = null, IMutation<DummyItem> mutation = null)
        {
            parent = parent ?? new DummyTestComposite<DummyItem>();
            mutation = mutation ?? new DummyMutation<DummyItem>();

            return new TestLeaf<DummyItem>(parent, mutation);
        }

        #endregion
    }
}
