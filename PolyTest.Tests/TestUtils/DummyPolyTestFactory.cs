using System;

namespace PolyTest.Tests.TestUtils
{
    internal class DummyPolyTestFactory : IPolyTestFactory
    {
        public IMutation<T> Mutation<T>(string mutationDescription, Action<T> mutationToApply)
        {
            throw new NotImplementedException();
        }


        public Func<string, Func<ClassToTest>, ITestComposite<ClassToTest>> StubbedRoot { get; set; }

        public ITestComposite<T> Root<T>(string description, Func<T> setup)
        {
            return (ITestComposite<T>)StubbedRoot(description, setup as Func<ClassToTest>);
        }

        public ITestComposite<T> Composite<T>(ITestComposite<T> parent, IMutation<T> mutation)
        {
            throw new NotImplementedException();
        }

        public ITestComponent<T> Leaf<T>(ITestComposite<T> parent, IMutation<T> mutation)
        {
            throw new NotImplementedException();
        }
    }
}