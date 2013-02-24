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
        public static ITestComposite<T> Change<T>(this ITestComposite<T> root, IMutation<T> mutation)
        {
            var composite = new  TestComposite<T>(root, mutation);
            root.Add(composite);
            return composite;
        }

        public static ITestComposite<T> Change<T>(this ITestComposite<T> root, string mutationDescription, Action<T> mutationAction)
        {
            return root.Change(Poly.Mutation(mutationDescription, mutationAction));
        }

        /// <summary>
        /// do not include this item when testing ... it is just a transition other states
        /// </summary>
        public static ITestComposite<T> IgnoreSelf<T>(this ITestComposite<T> composite, string reason = "")
        {
            composite.IncludeSelfInEnumeration = false;
            return composite;

        }
    }
}
