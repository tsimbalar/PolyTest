using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyTest.Tests.Composites.Fluent
{

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

        ITestCompositeFluent<T> Consider(IMutation<T> mutation,
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

        public ITestCompositeFluent<T> Consider(IMutation<T> mutation, bool includeMutationInTestCase, Func<ITestCompositeNestedFluent<T>, ITestCompositeFluent<T>> nestedAdd)
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

        public static ITestCompositeFluent<T> Consider<T>(this ITestCompositeFluent<T> tree,
            string mutationDescription, Action<T> mutationAction,
            Func<ITestCompositeNestedFluent<T>, ITestCompositeFluent<T>> nestedAdd)
        {
            return tree.Consider(new Mutation<T>(mutationDescription, mutationAction), true, nestedAdd);
        }
    }
}
