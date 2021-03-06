using System;

namespace PolyTest
{
    /// <summary>
    /// What is a test case ? 
    /// Just a description and an initial state
    /// </summary>
    /// <typeparam name="T">The type that is initialized by the testcase</typeparam>
    public interface ITestCase<out T>
    {
        /// <summary>
        /// A human-driendly description of the state we reach with this Initialization
        /// </summary>
        String Description { get; }

        /// <summary>
        /// Initialize an instance of <typeparamref name="T"/> for testing
        /// </summary>
        /// <returns></returns>
        T Arrange();
    }
}