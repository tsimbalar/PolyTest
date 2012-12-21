using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PolyTest.Tests.TestUtils
{
    public static class DummyAssert
    {
        /// <summary>
        /// Stupid custom Assert method
        /// </summary>
        public static void AssertIsNotFive(int actual, string extraInfo = null)
        {
            Assert.AreNotEqual(5, actual, extraInfo);
        }

    }
}
