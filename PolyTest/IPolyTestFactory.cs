using System;

namespace PolyTest
{
    public interface IPolyTestFactory
    {
        IMutation<T> Mutation<T>(string mutationDescription, Action<T> mutationToApply);
        ITestComposite<T> Root<T>(string description, Func<T> setup);
        ITestComposite<T> Composite<T>(ITestComposite<T> parent, IMutation<T> mutation);
        ITestComponent<T> Leaf<T>(ITestComposite<T> parent, IMutation<T> mutation);
    }
}