using System;
using PolyTest.Implementations;

namespace PolyTest.Fluent
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
}