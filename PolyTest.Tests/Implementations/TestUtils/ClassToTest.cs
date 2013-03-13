namespace PolyTest.Tests.Implementations.TestUtils
{
    internal class ClassToTest
    {
        public ClassToTest(int initialValue)
        {
            IntProperty = initialValue;
        }

        public int IntProperty { get; set; }
    }
}