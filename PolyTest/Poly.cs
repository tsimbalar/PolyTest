using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PolyTest.Fluent;

namespace PolyTest
{
    /// <summary>
    /// Entry point to interact with PolyTest basic components
    /// This somehow acts as a static Factory
    /// </summary>
    public static class Poly
    {
        private static readonly IPolyTestFactory FactoryInstance = new DefaultPolyTestFactory();
        private static readonly IPolyTestRunner RunnerInstance = new DefaultPolyTestRunner();

        public static IPolyTestFactory Create
        {
            get { return FactoryInstance; }
        }

        public static IPolyTestRunner Runner
        {
            get { return RunnerInstance; }
        }
    }
}
