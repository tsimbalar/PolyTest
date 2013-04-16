using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PolyTest.Fluent;

namespace PolyTest.Implementations.Fluent
{
    internal class TestExecutionReport<T> : ITestExecutionReport<T>
    {
        private readonly List<TestResult<T>> _results;

        internal TestExecutionReport()
        {
            _results = new List<TestResult<T>>();
        }

        internal void Add(TestResult<T> test)
        {
            _results.Add(test);
        }


        public int Count { get { return _results.Count; } }
        private IEnumerable<ITestResult<T>> All { get { return _results.AsReadOnly(); } }
        public IEnumerable<ITestResult<T>> Passed { get { return All.Where(t => t.IsSuccess); } }
        public IEnumerable<ITestResult<T>> Failed { get { return All.Where(t => !t.IsSuccess); } }
        /// <summary>
        /// Assert that all tests passed.
        /// </summary>
        /// <exception cref="TestExecutionAssertFailedException">when a test or more did not pass</exception>
        public void AssertAllPassed()
        {
            var failureSummary = FailureSummary;
            if (failureSummary != null)
            {
                throw new TestExecutionAssertFailedException(failureSummary);
            }
        }

        /// <summary>
        /// Assert that all tests passed
        /// </summary>
        /// <param name="handleFailureSummary">action to execute if some tests did not passed. Will be passed a string which is a summary of the failing tests (report)</param>
        public void AssertAllPassed(Action<string> handleFailureSummary)
        {
            var failureSummary = FailureSummary;
            if (failureSummary != null)
            {
                handleFailureSummary(failureSummary);
            }
        }

        private string FailureSummary
        {
            get
            {
                // Make a detailed message with all the failures ....
                var failures = Failed.ToList();
                if (failures.Any())
                {
                    var message = new StringBuilder();
                    message.AppendFormat("{0}/{1} TestCases failed: \n", failures.Count, Count);
                    foreach (var testResult in failures)
                    {
                        message.AppendFormat("-{0}\n", testResult);
                    }
                    return message.ToString();
                }
                else
                {
                    return null;
                }
            }
        }


        public IEnumerator<ITestResult<T>> GetEnumerator()
        {
            return All.GetEnumerator();
        }

        public override string ToString()
        {
            return string.Join("\n", _results);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}