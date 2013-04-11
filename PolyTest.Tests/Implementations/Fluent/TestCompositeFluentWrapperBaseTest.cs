using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolyTest.Implementations;
using PolyTest.Implementations.Fluent;
using PolyTest.Tests.TestUtils;
using PolyTest.Tests.TestUtils.Fluent;
using Xunit;

namespace PolyTest.Tests.Implementations.Fluent
{
    /// <summary>
    /// Tests on the base class on behaviours that subclasses will get for free 
    /// </summary>
    public class TestCompositeFluentWrapperBaseTest
    {

        [Fact]
        public void Ctor_with_wrapped_null_throws_ArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                    new DummySubClassOfTestCompositeFluentWrapperBase<ClassToTest>(null)
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
        public void IncludeSelfInEnumeration_gets_Wrapped_IncludeSelfInEnumeration()
        {
            // Arrange
            var wrapped = new DummyTestComposite<ClassToTest>();
            wrapped.IncludeSelfInEnumeration = true;
            var sut = MakeSut(wrapped);

            // Act
            var actual = sut.ExposedIncludeSelfInEnumeration;

            // Assert
            Assert.Equal(wrapped.IncludeSelfInEnumeration, actual);
        }

        [Fact]
        public void IncludeSelfInEnumeration_sets_Wrapped_IncludeSelfInEnumeration()
        {
            // Arrange
            var wrapped = new DummyTestComposite<ClassToTest>();
            var newValue = true;
            wrapped.IncludeSelfInEnumeration = !newValue;

            var sut = MakeSut(wrapped);

            // Act
            sut.ExposedIncludeSelfInEnumeration = newValue;

            // Assert
            Assert.Equal(newValue, wrapped.IncludeSelfInEnumeration);
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
        public void Consider_adds_TestComposite_to_wrapped()
        {
            // Arrange
            var wrapped = new DummyTestComposite<ClassToTest>();
            var sut = MakeSut(wrapped);
            var mutationToAdd = new DummyMutation<ClassToTest>();

            // Act
            sut.Consider(mutationToAdd);

            // Assert
            Assert.IsType<TestComposite<ClassToTest>>(wrapped.Children.Single());
            var actualChild = (TestComposite<ClassToTest>)wrapped.Children.Single();
            Assert.Equal(wrapped.Description + " AND " + mutationToAdd.Description, actualChild.Description);
        }

        [Fact]
        public void Consider_with_nested_returns_self()
        {
            // Arrange
            var sut = MakeSut();
            var mutationToAdd = new DummyMutation<ClassToTest>();

            // Act
            var actual = sut.Consider(mutationToAdd, c => c);

            // Assert
            Assert.Same(sut, actual);
        }

        [Fact]
        public void Consider_with_nested_adds_Nested_children()
        {
            // Arrange
            var wrapped = new DummyTestComposite<ClassToTest>();
            var sut = MakeSut(wrapped);
            var mutationToAdd = new DummyMutation<ClassToTest>();
            var nestedMutation = new DummyMutation<ClassToTest>();

            // Act
            sut.Consider(mutationToAdd, c => c.Consider(nestedMutation));

            // Assert
            Assert.IsType<TestComposite<ClassToTest>>(wrapped.Children.Single());
            var actualChild = (TestComposite<ClassToTest>)wrapped.Children.Single();
            Assert.Equal(wrapped.Description + " AND " + mutationToAdd.Description, actualChild.Description);
            var grandChild = (TestComposite<ClassToTest>)actualChild.Children.Single();
            Assert.Equal(wrapped.Description + " AND " + mutationToAdd.Description + " AND " + nestedMutation.Description, grandChild.Description);
        }

        [Fact]
        public void Consider_with_nestedAdd_returning_null_throws_InvalidOperationException()
        {
            // Arrange
            var sut = MakeSut();
            var mutationToAdd = new DummyMutation<ClassToTest>();

            // Act & Assert
            var actualException = Assert.Throws<InvalidOperationException>(() =>
                             sut.Consider(mutationToAdd, t => null)
                );
            Assert.Equal("nestedAdd returned null", actualException.Message);
        }

        [Fact]
        public void Consider_with_nestedAdd_returning_not_TestCompositeFluentWrapperBase_throws_InvalidOperationException()
        {
            // Arrange
            var sut = MakeSut();
            var mutationToAdd = new DummyMutation<ClassToTest>();
            var nestedAddResult = new DummyTestCompositeFluent<ClassToTest>();

            // Act & Assert
            var actualException = Assert.Throws<InvalidOperationException>(() =>
                             sut.Consider(mutationToAdd, t => nestedAddResult)
                );
            var expectedMessage =
                String.Format("Expected nestedAdd to return an instance of type {0}. It returned : {1}",
                              typeof(TestCompositeFluentWrapperBase<ClassToTest>), typeof(DummyTestCompositeFluent<ClassToTest>)
                    );
            Assert.Equal(expectedMessage, actualException.Message);
        }

        [Fact]
        public void Walk_calls_act_for_each_element_in_wrapped_Enumerate()
        {
            // Arrange
            var wrappedTestCases = MakeManyTestCases();
            var wrapped = new DummyTestComposite<ClassToTest>();
            wrapped.TestCases.AddRange(wrappedTestCases);

            var sut = MakeSut(wrapped);
            var actCallCounter = 0;

            // Act
            var actual = sut.Walk(act: c =>
                              {
                                  actCallCounter = actCallCounter + 1;
                                  return c;
                              },
                     assert: c => { }); // nothing to assert in this test

            // Assert
            Assert.Equal(wrappedTestCases.Count, actCallCounter);
        }

        [Fact]
        public void Walk_calls_assert_for_each_element_in_wrapped_Enumerate()
        {
            // Arrange
            var wrappedTestCases = MakeManyTestCases();
            var wrapped = new DummyTestComposite<ClassToTest>();
            wrapped.TestCases.AddRange(wrappedTestCases);

            var sut = MakeSut(wrapped);
            var assertCallCounter = 0;

            // Act
            var actual = sut.Walk(
                    act: c => c,
                    assert: c => assertCallCounter++); // nothing to assert in this test

            // Assert
            Assert.Equal(wrappedTestCases.Count, assertCallCounter);
        }

        [Fact]
        public void Walk_returns_a_TestExecutionReport()
        {
            // Arrange
            var wrappedTestCases = MakeManyTestCases();
            var wrapped = new DummyTestComposite<ClassToTest>();
            wrapped.TestCases.AddRange(wrappedTestCases);
            var sut = MakeSut(wrapped);

            // Act
            var actual = sut.Walk(
                act: c => c,
                assert: c => { }
                );

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<TestExecutionReport<ClassToTest>>(actual);
        }

        [Fact]
        public void Walk_returns_one_result_for_each_testCase()
        {
            // Arrange
            var wrappedTestCases = MakeManyTestCases();
            var wrapped = new DummyTestComposite<ClassToTest>();
            wrapped.TestCases.AddRange(wrappedTestCases);
            var sut = MakeSut(wrapped);

            // Act
            var actual = sut.Walk(
                act: c => c,
                assert: c => { }
                );

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(wrappedTestCases.Count, actual.Count);
            Assert.Equal(wrappedTestCases.Select(tc => tc.Description), actual.All.Select(tr=> tr.TestCase.Description));
        }



        #region Test Helper Methods

        private static DummySubClassOfTestCompositeFluentWrapperBase<ClassToTest> MakeSut(ITestComposite<ClassToTest> wrapped = null)
        {
            wrapped = wrapped ?? new DummyTestComposite<ClassToTest>();
            return new DummySubClassOfTestCompositeFluentWrapperBase<ClassToTest>(wrapped);
        }

        /// <summary>
        /// Compare 2 lists of test cases .. can be of different types, but we look at the inside ...
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        private static void AssertAreEquivalent<T>(IEnumerable<ITestCase<T>> expected, IEnumerable<ITestCase<T>> actual)
        {
            // Just look a the Description to see if it's the same item
            Assert.Equal(
                expected.Select(t => t.Description),
                actual.Select(t => t.Description));
        }

        private static DummyTestCase<ClassToTest> MakeTestCase(int identifier)
        {
            var testCase = new DummyTestCase<ClassToTest>(String.Format("test case {0}", identifier));
            testCase.StubbedArrange = () => new ClassToTest(identifier);
            return testCase;
        }

        private static List<DummyTestCase<ClassToTest>> MakeManyTestCases(int count = 3)
        {
            var result = new List<DummyTestCase<ClassToTest>>();
            for (int i = 0; i < count; i++)
            {
                var indexForDisplay = i + 1;
                var testCase = MakeTestCase(indexForDisplay);
                result.Add(testCase);
            }
            return result;
        } 

        /// <summary>
        /// We cannot test an abstract class, so we test a direct subclass
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class DummySubClassOfTestCompositeFluentWrapperBase<T> : TestCompositeFluentWrapperBase<T>
        {
            public DummySubClassOfTestCompositeFluentWrapperBase(ITestComposite<T> wrapped)
                : base(wrapped)
            {
            }

            /// <summary>
            /// Make the protected property visible in the test
            /// </summary>
            public bool ExposedIncludeSelfInEnumeration
            {
                get { return IncludeSelfInEnumeration; }
                set { IncludeSelfInEnumeration = value; }
            }
        }

        #endregion

    }

}
