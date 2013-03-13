namespace PolyTest.Tests.ApiUsage
{
    public class DummyItem
    {
        public DummyItem(int initial, bool initialBool = true)
        {
            IntProperty = initial;
            BoolProperty = initialBool;
        }

        public int IntProperty { get; set; }

        public bool BoolProperty { get; set; }

    }
}