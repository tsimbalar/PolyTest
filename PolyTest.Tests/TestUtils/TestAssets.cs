using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PolyTest.Tests.TestUtils
{
    public static class TestAssets
    {

        #region Sut to test our tests (sic)

        public class DummyItem
        {
            public DummyItem(int initial)
            {
                IntProperty = initial;
                BoolProperty = false;
            }

            public int IntProperty { get; set; }

            public bool BoolProperty { get; set; }

        }

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

        /// <summary>
        /// Stupid custom Assert method
        /// </summary>
        public static void AssertIsNotFive(int actual, string extraInfo)
        {
            Assert.AreNotEqual(5, actual, extraInfo);
        }

        #endregion
    }
}
