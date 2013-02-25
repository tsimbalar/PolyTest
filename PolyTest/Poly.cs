using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolyTest.Implementations;

namespace PolyTest
{
    /// <summary>
    /// Entry point to interact with PolyTest basic components
    /// </summary>
    public static class Poly
    {

        public static IMutation<T> Mutation<T>(string mutationDescription, Action<T> mutationToApply)
        {
            return new Mutation<T>(mutationDescription, mutationToApply);
        }

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
