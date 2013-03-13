namespace PolyTest.Tests.Implementations
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