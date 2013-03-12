

using Xunit;

namespace PolyTest.Tests.ApiUsage
{
    public static class DummyAssert
    {
        /// <summary>
        /// Stupid custom Assert method
        /// </summary>
        public static void AssertIsNotFive(int actual, string extraInfo = null)
        {
            Assert.NotEqual(5, actual);
        }

    }
}
