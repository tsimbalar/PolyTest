using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyTest
{
    /// <summary>
    /// Entry point to interact with PolyTest
    /// </summary>
    public static class Poly
    {

        public static IMutation<T> Mutation<T>(string mutationDescription, Action<T> mutationToApply)
        {
            return new Mutation<T>(mutationDescription, mutationToApply);
        }
    }
}
