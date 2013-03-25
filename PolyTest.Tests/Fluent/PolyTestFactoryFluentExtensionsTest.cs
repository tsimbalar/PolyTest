using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolyTest.Implementations.Fluent;
using PolyTest.Tests.TestUtils;
using Xunit;
using PolyTest.Fluent;

namespace PolyTest.Tests.Fluent
{
    public class PolyTestFactoryFluentExtensionsTest
    {
        [Fact]
        public void From_returns_wrapper_around_factory_Root()
        {
            // Arrange
            var sut = MakeSut();
            var returnedRoot = new DummyTestComposite<ClassToTest>();
            sut.StubbedRoot = (desc, setup) => returnedRoot;

            // Act
            var actual = sut.From<ClassToTest>("initial", () => new ClassToTest(5));

            // Assert
            Assert.IsType<TestRootFluentWrapper<ClassToTest>>(actual);
            var asWrapper = actual as TestRootFluentWrapper<ClassToTest>;
            Assert.Same(returnedRoot, asWrapper.Wrapped); // took the root from Factory class
        }

        [Fact]
        public void From_calls_Factory_Root_with_passed_parameters()
        {
            // Arrange
            var initialStateDescription = "some initial state";
            Func<ClassToTest> setupFunction = () => new ClassToTest(5);
            var sut = MakeSut();
            string actualDescriptionPassedTRoot = "";
            Func<ClassToTest> actualSetUpPassedToRoot = () => new ClassToTest(0);
            sut.StubbedRoot = (desc, setup) =>
            {
                actualDescriptionPassedTRoot = desc; // record which parameters were passed
                actualSetUpPassedToRoot = setup;
                return new DummyTestComposite<ClassToTest>(); ;
            };

            // Act
            var actual = sut.From<ClassToTest>(initialStateDescription, setupFunction);

            // Assert
            Assert.Equal(initialStateDescription, actualDescriptionPassedTRoot);
            Assert.Same(setupFunction, actualSetUpPassedToRoot);
        }


        private static DummyPolyTestFactory MakeSut()
        {
            return new DummyPolyTestFactory();
        }
    }
}
