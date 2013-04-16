using System;
using System.Collections.Generic;
using System.Linq;
using PolyTest.Fluent;
using PolyTest.Implementations.Fluent;

namespace PolyTest
{
    internal class DefaultPolyTestRunner : IPolyTestRunner
    {
        public ITestExecutionReport<TResult> Execute<T, TResult>(IEnumerable<ITestCase<T>> testCases, Func<T, TResult> act, Action<TResult> assert)
        {
            var report = new TestExecutionReport<TResult>();
            foreach (var testCase in testCases.Select((tc, i) => new { TestCase = tc, Index = i }))
            {
                var result = TestRunner.Run(testCase.Index, testCase.TestCase, act, assert);
                report.Add(result);
            }
            return report;
        }
    }
}