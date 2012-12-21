using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PolyTest.Tests.TestUtils
{
    #region Sut to test our tests (sic)

    public class FakeSut
        {
            public int DoIt(DummyItem item)
            {
                return item.IntProperty;
            }

            public bool HasIt(DummyItem item)
            {
                return item.BoolProperty;
            }
        }

        
        #endregion
}
