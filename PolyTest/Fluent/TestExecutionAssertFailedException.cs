using System;
using System.Runtime.Serialization;

namespace PolyTest.Fluent
{
    internal class TestExecutionAssertFailedException : Exception
    {
        public TestExecutionAssertFailedException(string message)
            : base(message)
        {
        }

        public TestExecutionAssertFailedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected TestExecutionAssertFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}