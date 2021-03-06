using System;
using System.Collections.Generic;

namespace PolyTest.Fluent
{
    public interface ITestExecutionReport<out T> : IEnumerable<ITestResult<T>>, IFluentInterface
    {
        int Count { get; }
        IEnumerable<ITestResult<T>> Passed { get; }
        IEnumerable<ITestResult<T>> Failed { get; }
        void AssertAllPassed();
        void AssertAllPassed(Action<string> handleFailureSummary );
    }
}