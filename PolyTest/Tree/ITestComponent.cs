using System.Collections.Generic;

namespace PolyTest.Tree
{
    /// <summary>
    /// The component that is the common part of every element in the tree
    /// </summary>
    /// <typeparam name="T">the type that the test cases initialize</typeparam>
    public interface ITestComponent<out T> : ITestCase<T>
    {
        IEnumerable<ITestCase<T>> Enumerate();

        IEnumerable<ITestComponent<T>> Children { get; }
    }
}