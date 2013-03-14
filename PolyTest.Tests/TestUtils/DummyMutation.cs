using System;

namespace PolyTest.Tests.TestUtils
{
    public class DummyMutation<T> : IMutation<T>
    {
        private readonly string _description;

        public DummyMutation(string description = "dummy mutation description")
        {
            _description = description;
        }

        public string Description { get { return _description; } }
        public void Apply(T source)
        {
            StubbedApply(source);
        }

        public Action<T> StubbedApply { get; set; }
    }
}