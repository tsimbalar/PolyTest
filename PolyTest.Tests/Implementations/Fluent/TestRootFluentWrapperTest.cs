using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolyTest.Implementations.Fluent;
using PolyTest.Tests.TestUtils;
using Xunit;

namespace PolyTest.Tests.Implementations.Fluent
{
    public class TestRootFluentWrapperTest
    {
        // Just test that it is indeed a subclass of TestCompositeFluentWrapperBase, 
        // so all the tests we already do on TestCompositeFluentWrapperBase are valid
        [Fact]
        public void Sut_with_ConditionToTest_is_subclass_of_TestCompositeFluentWrapperBase()
        {
            // Arrange
            
            // Act
            var sut = MakeSut();

            // Assert
            Assert.IsAssignableFrom<TestCompositeFluentWrapperBase<ClassToTest>>(sut);
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

        #region Test Helper Methods

        private TestRootFluentWrapper<ClassToTest> MakeSut(ITestComposite<ClassToTest> wrapped = null)
        {
            wrapped = wrapped ?? new DummyTestComposite<ClassToTest>();
            return new TestRootFluentWrapper<ClassToTest>(wrapped);
        }

        #endregion
    }
}
