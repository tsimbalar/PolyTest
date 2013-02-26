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
        private static readonly IPolyTestFactory _factory = new PolyTestFactory();  
        public static IPolyTestFactory Factory
        {
            get { return _factory; }
        }
    }
}
