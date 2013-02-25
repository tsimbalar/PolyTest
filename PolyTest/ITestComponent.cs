using System.Collections.Generic;

namespace PolyTest
{
    /// <summary>
    /// The component that is the common part of every element in the tree
    /// </summary>
    /// <typeparam name="T">the type that the test cases initialize</typeparam>
    public interface ITestComponent<out T> : ITestCase<T>
    {
        /// <summary>
        /// Iterate over the list of test cases represented by this component
        /// </summary>
        /// <returns>a IEnumerable with all possible test cases starting from this component, and applying all children mutations</returns>
        IEnumerable<ITestCase<T>> Enumerate();

        /// <summary>
        /// The children of this component in the tree of test cases
        /// </summary>
        IEnumerable<ITestComponent<T>> Children { get; }
    }
}