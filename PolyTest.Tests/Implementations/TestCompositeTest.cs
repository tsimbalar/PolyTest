using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolyTest.Implementations;
using PolyTest.Tests.TestUtils;
using Xunit;

namespace PolyTest.Tests.Implementations
{
    public class TestCompositeTest
    {
        [Fact]
        public void Ctor_with_parent_null_throws_ArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                             new TestComposite<ClassToTest>(null, new DummyMutation<ClassToTest>("the mutation"))
                );
        }

        [Fact]
        public void Ctor_with_mutation_null_throws_ArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                             new TestComposite<ClassToTest>(new DummyTestComposite<ClassToTest>("parent"), null)
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
        public void IncludeSelfInEnumeration_is_writable()
        {
            // Arrange
            var newValue = false;
            ITestComposite<ClassToTest> sut = MakeSut();

            // Act
            sut.IncludeSelfInEnumeration = newValue;

            // Assert
            Assert.Equal(newValue, sut.IncludeSelfInEnumeration);
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
        public void Enumerate_with_IncludeSelfInEnumeration_false_enumerates_over_children_and_children_Enumerate()
        {
            // Arrange
            var sut = MakeSut();
            ((ITestComposite<ClassToTest>)sut).IncludeSelfInEnumeration = false;
            var child = new DummyTestComponent<ClassToTest>();
            sut.Add(child);

            // Act
            var actual = sut.Enumerate();

            // Assert
            Assert.Equal(child.Enumerate(), actual);
        }

        [Fact]
        public void Enumerate_with_IncludeSelfInEnumeration_true_enumerates_over_sut_and_children_and_children_Enumerate()
        {
            // Arrange
            var sut = MakeSut();
            ((ITestComposite<ClassToTest>)sut).IncludeSelfInEnumeration = true;
            var child = new DummyTestComponent<ClassToTest>();
            sut.Add(child);

            // Act
            var actual = sut.Enumerate();

            // Assert
            Assert.Equal(Enumerable.Repeat(sut, 1).Union(child.Enumerate()), actual);
        }

        #region Test Helper Methods

        private static TestComposite<ClassToTest> MakeSut(ITestComposite<ClassToTest> parent = null, IMutation<ClassToTest> mutation = null  )
        {
            parent = parent ?? new DummyTestComposite<ClassToTest>();
            mutation = mutation ?? new DummyMutation<ClassToTest>();

            return new TestComposite<ClassToTest>(parent, mutation);
        }

        #endregion
    }
}
