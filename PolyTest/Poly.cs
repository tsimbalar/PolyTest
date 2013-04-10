using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyTest
{
    /// <summary>
    /// Entry point to interact with PolyTest basic components
    /// This somehow acts as a static Factory
    /// </summary>
    public static class Poly
    {
        private static readonly IPolyTestFactory FactoryInstance = new DefaultPolyTestFactory();  
        public static IPolyTestFactory Create
        {
            get { return FactoryInstance; }
        }
    }
}
