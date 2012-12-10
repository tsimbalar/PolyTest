using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolyTest.Tests.TestUtils;

namespace PolyTest.Tests.Composites.Fluent
{
    [TestClass]
    public class CompositeFluentTest
    {
        [TestMethod]
        public void FluentTest()
        {
            // Arrange
            var sut = new TestAssets.FakeSut();

            TestTree.From("starting with 5", () => new TestAssets.DummyItem(5))
                 .Consider("add 2", d => { d.IntProperty = d.IntProperty + 2; })
                 .Consider("add 4", d => { d.IntProperty = d.IntProperty + 4; })
                 .Consider("add 1", d => { d.IntProperty++; },
                    opt => opt
                        .Consider("add 13", d => { d.IntProperty = d.IntProperty + 13; })
                        .Consider("remove 3", d => { d.IntProperty = d.IntProperty - 3; })
                 )
                 .GoThrough("add 0", d => { },
                    opt => opt
                        .IgnoreSelf()
                        .Consider("add 1", d => { d.IntProperty++; })
                        .Consider("remove 1", d => { d.IntProperty = d.IntProperty - 1; })
                        .GoThrough("add 3", d => { d.IntProperty += 3; },
                            opt2 => opt2
                                .Consider("add 4", d => { d.IntProperty += 4; })
                                .Consider("remove 2", d => { d.IntProperty -= 2; })
                        )
                 )
                 .Walk((descr, arrange) =>
                            {
                                var init = arrange();
                                sut.DoIt(init);
                                TestAssets.AssertIsNotFive(init.IntProperty, descr);
                            })
                 ;
        }



    }

    public static class TestTree
    {
        /// <summary>
        /// Entry point for the Fluent interface
        /// </summary>
        public static ITestCompositeFluent<T> From<T>(string initialStateDescription, Func<T> setup)
        {
            return new TestCompositeFluentWrapper<T>(new TestRoot<T>(initialStateDescription, setup));
        }
    }

    public interface ITestCompositeFluent<T> : ITestComposite<T>
    {
        ITestCompositeFluent<T> Consider(IMutation<T> mutation);

        ITestCompositeFluent<T> GoThrough(IMutation<T> mutation,
                                          bool includeMutationInTestCase,
                                          Func<ITestCompositeNestedFluent<T>, ITestCompositeFluent<T>> nestedAdd);
    }

    public interface ITestCompositeNestedFluent<T> : ITestCompositeFluent<T>
    {
        ITestCompositeNestedFluent<T> IgnoreSelf();
        ITestCompositeNestedFluent<T> IncludeSelf();
    }

    internal class TestCompositeFluentWrapper<T> : ITestCompositeFluent<T>
    {
        private readonly ITestComposite<T> _wrapped;

        public TestCompositeFluentWrapper(ITestComposite<T> wrapped)
        {
            if (wrapped == null) throw new ArgumentNullException("wrapped");
            _wrapped = wrapped;
        }

        public string Description { get { return _wrapped.Description; } }
        public T Arrange()
        {
            return _wrapped.Arrange();
        }

        public IEnumerable<ITestCase<T>> Enumerate()
        {
            return _wrapped.Enumerate();
        }

        public void Add(ITestComponent<T> child)
        {
            _wrapped.Add(child);
        }

        public bool IncludeSelfInEnumeration
        {
            get { return _wrapped.IncludeSelfInEnumeration; }
            set { _wrapped.IncludeSelfInEnumeration = value; }
        }

        public ITestCompositeFluent<T> Consider(IMutation<T> mutation)
        {
            this.Add(new TestComposite<T>(this, mutation, true));
            return this;
        }

        public ITestCompositeFluent<T> GoThrough(IMutation<T> mutation, bool includeMutationInTestCase, Func<ITestCompositeNestedFluent<T>, ITestCompositeFluent<T>> nestedAdd)
        {
            ITestCompositeNestedFluent<T> composite = new TestCompositeFluentNestedWrapper<T>(new TestComposite<T>(this, mutation, includeInEnumeration: includeMutationInTestCase));
            var updatedComposite = nestedAdd(composite);

            this.Add(updatedComposite);

            return this;
        }
    }

    internal class TestCompositeFluentNestedWrapper<T> : TestCompositeFluentWrapper<T>, ITestCompositeNestedFluent<T>
    {
        public TestCompositeFluentNestedWrapper(ITestComposite<T> wrapped)
            : base(wrapped)
        {
        }

        public ITestCompositeNestedFluent<T> IgnoreSelf()
        {
            this.IncludeSelfInEnumeration = false;
            return this;
        }

        public ITestCompositeNestedFluent<T> IncludeSelf()
        {
            this.IncludeSelfInEnumeration = true;
            return this;
        }
    }

    public static class TestComponentExtensions
    {
        public static ITestCompositeFluent<T> Consider<T>(this ITestCompositeFluent<T> tree,
            string mutationDescr, Action<T> mutationAction)
        {
            return tree.Consider(new Mutation<T>(mutationDescr, mutationAction));
        }

        public static ITestCompositeFluent<T> GoThrough<T>(this ITestCompositeFluent<T> tree,
            string mutationDescription, Action<T> mutationAction,
            Func<ITestCompositeNestedFluent<T>, ITestCompositeFluent<T>> nestedAdd)
        {
            return tree.GoThrough(new Mutation<T>(mutationDescription, mutationAction), false, nestedAdd);
        }

        public static ITestCompositeFluent<T> Consider<T>(this ITestCompositeFluent<T> tree,
            string mutationDescription, Action<T> mutationAction,
            Func<ITestCompositeNestedFluent<T>, ITestCompositeFluent<T>> nestedAdd)
        {
            return tree.GoThrough(new Mutation<T>(mutationDescription, mutationAction), true, nestedAdd);
        }
    }
}
