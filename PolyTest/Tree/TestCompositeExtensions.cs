using System;

namespace PolyTest.Tree
{
    /// <summary>
    /// Extensions to walk over the testcases of a tree of testcases, providing access to initialization method and description of the testcase
    /// </summary>
    public static class TestCompositeExtensions
    {
        public static void Walk<T>(this ITestComponent<T> component, Action<ITestCase<T>> action)
        {
            foreach (var testCase in component.Enumerate())
            {
                ITestCase<T> currentCase = testCase; // copy of the loop variable because it is referenced in a closure
                action(currentCase);
            }
        }
    }
}