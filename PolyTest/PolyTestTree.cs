using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolyTest.Tree;

namespace PolyTest
{
    public static class PolyTestTree
    {
        public static ITestComposite<T> Root<T>(string description, Func<T> setup)
        {
            return new TestRoot<T>(description, setup);
        }

        public static ITestComponent<T> Leaf<T>(ITestComposite<T> parent, IMutation<T> mutation)
        {
            return new TestLeaf<T>(parent, mutation);
        }

        public static ITestComposite<T> Composite<T>(ITestComposite<T> parent, IMutation<T> mutation)
        {
            return new TestComposite<T>(parent, mutation);
        }
    }
}
