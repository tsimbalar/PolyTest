using System;
using PolyTest.Implementations;

namespace PolyTest
{
    internal class DefaultPolyTestFactory : IPolyTestFactory
    {
        public virtual IMutation<T> Mutation<T>(string mutationDescription, Action<T> mutationToApply)
        {
            return new Mutation<T>(mutationDescription, mutationToApply);
        }

        public virtual ITestComposite<T> Root<T>(string description, Func<T> setup)
        {
            return new TestRoot<T>(description, setup);
        }

        public virtual ITestComposite<T> Composite<T>(ITestComposite<T> parent, IMutation<T> mutation)
        {
            return new TestComposite<T>(parent, mutation);
        }

        public virtual ITestComponent<T> Leaf<T>(ITestComposite<T> parent, IMutation<T> mutation)
        {
            return new TestLeaf<T>(parent, mutation);
        }
    }
}