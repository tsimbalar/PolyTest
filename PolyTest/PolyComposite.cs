using System;
using PolyTest.Implementations;

namespace PolyTest
{
    public static class PolyComposite
    {
        public static ITestComposite<T> Root<T>(string description, Func<T> setup)
        {
            return new TestRoot<T>(description, setup);
        }

        public static ITestComposite<T> Composite<T>(ITestComposite<T> parent, IMutation<T> mutation)
        {
            return new TestComposite<T>(parent, mutation);
        }

        public static ITestComponent<T> Leaf<T>(ITestComposite<T> parent, IMutation<T> mutation)
        {
            return new TestLeaf<T>(parent, mutation);
        }
    }
}