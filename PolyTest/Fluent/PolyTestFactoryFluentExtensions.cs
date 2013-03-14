using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolyTest.Implementations.Fluent;

namespace PolyTest.Fluent
{
    public static class PolyTestFactoryFluentExtensions
    {
        /// <summary>
        /// Entry point for the Fluent interface
        /// </summary>
        public static ITestRootFluent<T> From<T>(this IPolyTestFactory factory, string initialStateDescription, Func<T> setup)
        {
            return new TestRootFluentWrapper<T>(factory.Root(initialStateDescription, setup));
        }
    }
}
