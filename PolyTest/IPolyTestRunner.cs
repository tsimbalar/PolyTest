using System;
using System.Collections.Generic;
using PolyTest.Fluent;

namespace PolyTest
{
    public interface IPolyTestRunner
    {
        ITestExecutionReport<TResult> Execute<T, TResult>(IEnumerable<ITestCase<T>> testCases, Func<T, TResult> act, Action<TResult> assert);
    }
}