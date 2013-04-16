using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyTest.Fluent
{
    public static class EnumerableOfITestCaseExtensions
    {
        public static ITestExecutionReport<TResult> Test<T, TResult>(this IEnumerable<ITestCase<T>> testCases, Func<T, TResult> act, Action<TResult> assert)
        {
            return Poly.Runner.Execute(testCases, act, assert);
        }
    }
}
