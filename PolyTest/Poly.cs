using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolyTest.Tree;

namespace PolyTest
{
    /// <summary>
    /// Entry point to interact with PolyTest
    /// </summary>
    public static class Poly
    {
        public static ITestComposite<T> TestRoot<T>(string description, Func<T> setup)
        {
            return new TestRoot<T>(description, setup);
        }

        public static IMutation<T> Mutation<T>(string mutationDescription, Action<T> mutationToApply)
        {
            return new Mutation<T>(mutationDescription, mutationToApply);
        }
    }
}
