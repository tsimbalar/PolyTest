namespace PolyTest.Tests.TestUtils
{
    internal class ClassToTest
    {
        public ClassToTest(int initialValue = 666)
        {
            IntProperty = initialValue;
        }

        public int IntProperty { get; set; }
    }
}