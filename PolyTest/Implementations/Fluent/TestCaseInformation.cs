using PolyTest.Fluent;

namespace PolyTest.Implementations.Fluent
{
    public class TestCaseInformation : ITestCaseInformation
    {
        private readonly int _index;
        private readonly string _description;

        public TestCaseInformation(int index, string description)
        {
            _index = index;
            _description = description;
        }

        public string Description { get { return _description; } }
        public int Index { get { return _index; } }
    }
}