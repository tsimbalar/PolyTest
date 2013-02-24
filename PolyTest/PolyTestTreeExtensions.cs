using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolyTest.Tree;

namespace PolyTest
{
    public static class PolyTestTreeExtensions
    {
        public static ITestComposite<T> Add<T>(this ITestComposite<T> root, IMutation<T> mutation)
        {
            var composite = new  TestComposite<T>(root, mutation);
            root.Add(composite);
            return composite;
        }

        public static ITestComposite<T> Add<T>(this ITestComposite<T> root, string mutationDescription, Action<T> mutationAction)
        {
            return root.Add(Poly.Mutation(mutationDescription, mutationAction));
        }
    }
}
