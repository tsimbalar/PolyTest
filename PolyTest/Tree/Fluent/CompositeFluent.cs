using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PolyTest.Tree.Fluent
{
    internal class TestCompositeFluentWrapper<T> : ITestCompositeFluent<T>
    {
        private readonly ITestComposite<T> _wrapped;

        public TestCompositeFluentWrapper(ITestComposite<T> wrapped)
        {
            if (wrapped == null) throw new ArgumentNullException("wrapped");
            _wrapped = wrapped;
        }

        public string Description { get { return _wrapped.Description; } }
        public T Arrange()
        {
            return _wrapped.Arrange();
        }

        public IEnumerable<ITestCase<T>> Enumerate()
        {
            return _wrapped.Enumerate();
        }

        public void Add(ITestComponent<T> child)
        {
            _wrapped.Add(child);
        }

        public bool IncludeSelfInEnumeration
        {
            get { return _wrapped.IncludeSelfInEnumeration; }
            set { _wrapped.IncludeSelfInEnumeration = value; }
        }

        public ITestCompositeFluent<T> Consider(IMutation<T> mutation)
        {
            this._wrapped.Add(new TestComposite<T>(this._wrapped, mutation));
            return this;
        }

        public ITestCompositeFluent<T> Consider(IMutation<T> mutation, Func<ITestCompositeNestedFluent<T>, ITestCompositeFluent<T>> nestedAdd)
        {
            ITestCompositeNestedFluent<T> composite = new TestCompositeFluentNestedWrapper<T>(new TestComposite<T>(this._wrapped, mutation));
            var updatedComposite = nestedAdd(composite);

            this._wrapped.Add(((TestCompositeFluentWrapper<T>)updatedComposite)._wrapped);

            return this;
        }

        public void ForEach(Action<ITestTreeNode<T>> action)
        {
            foreach (var testTreeNode in this.AsEnumerablePrivate())
            {
                action(testTreeNode);
            }
        }

        public IEnumerable<TResult> Select<TResult>(Func<ITestTreeNode<T>, TResult> selector)
        {
            return this.AsEnumerablePrivate().Select(selector);
        }

        public ITestExecutionReport<T> Walk<TResult>(Func<T, TResult> act, Action<TResult> assert)
        {
            var report = new TestExecutionReport<T>();
            foreach (var testCase in this.AsEnumerablePrivate())
            {
                report.Add(testCase.TestInternal(act, assert));
            }
            return report;
        }

        public IEnumerable<ITestTreeNode<T>> AsEnumerable()
        {
            return AsEnumerablePrivate();
        }

        private IEnumerable<TestTreeNode<T>> AsEnumerablePrivate()
        {
            var resultEnumerable = _wrapped.Enumerate().Select((tc, i) => new TestTreeNode<T>(i, tc));
            return resultEnumerable;
        }

    }

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
        public IEnumerable<ITestResult<T>> All { get { return _results.AsReadOnly(); } }
        public IEnumerable<ITestResult<T>> Passed { get { return All.Where(t => t.IsSuccess); } }
        public IEnumerable<ITestResult<T>> Failed { get { return All.Where(t => !t.IsSuccess); } }
        public void AssertAllPassed()
        {
            // Make a detailed message with all the failures ....
            var failures = Failed.ToList();
            if (failures.Any())
            {
                StringBuilder message = new StringBuilder();
                message.AppendFormat("{0}/{1} TestCases failed: \n", failures.Count, Count);
                foreach (var testResult in failures)
                {
                    message.AppendFormat("-{0}\n", testResult);
                }
                throw new TestExecutionAssertFailedException(message.ToString());
            }

        }


        public override string ToString()
        {
            return string.Join("\n", _results);
        }
    }

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


    internal class TestTreeNode<T> : ITestTreeNode<T>
    {
        private readonly int _index;
        private readonly ITestCase<T> _testCase;

        public TestTreeNode(int index, ITestCase<T> testCase)
        {
            if (testCase == null) throw new ArgumentNullException("testCase");
            _index = index;
            _testCase = testCase;
        }

        public string Description { get { return _testCase.Description; } }

        public int Index
        {
            get { return _index; }
        }

        public T Arrange()
        {
            return _testCase.Arrange();
        }


        internal TestResult<T, TResult> TestInternal<TResult>(Func<T, TResult> act, Action<TResult> assert)
        {
            try
            {
                var init = Arrange();
                try
                {
                    var result = act(init);

                    try
                    {
                        assert(result);
                    }
                    catch (Exception e)
                    {
                        return TestResult.AssertFailed(this, result, e);
                    }
                    return TestResult.Success(this, result);
                }
                catch (Exception e)
                {
                    return TestResult.ActFailed<T, TResult>(this, e);
                }
            }
            catch (Exception e)
            {
                return TestResult.ArrangeFailed<T, TResult>(this, e);
            }

        }

        public ITestResult<T, TResult> Test<TResult>(Func<T, TResult> act, Action<TResult> assert)
        {
            return TestInternal(act, assert);
        }

        public override string ToString()
        {
            return string.Format("#{0} - {1}", Index, Description);
        }
    }

    internal class TestResult
    {
        internal static TestResult<T, TResult> ArrangeFailed<T, TResult>(ITestTreeNode<T> testTreeNode, Exception exception)
        {
            return new TestResult<T, TResult>(testTreeNode, hasResult: false, arrangeException: exception);
        }

        internal static TestResult<T, TResult> ActFailed<T, TResult>(ITestTreeNode<T> testTreeNode, Exception exception)
        {
            return new TestResult<T, TResult>(testTreeNode, hasResult: false, actException: exception);
        }

        internal static TestResult<T, TResult> AssertFailed<T, TResult>(ITestTreeNode<T> testTreeNode, TResult result, Exception exception)
        {
            return new TestResult<T, TResult>(testTreeNode, hasResult: true, result: result, assertException: exception);
        }

        internal static TestResult<T, TResult> Success<T, TResult>(ITestTreeNode<T> testTreeNode, TResult result)
        {
            return new TestResult<T, TResult>(testTreeNode, hasResult: true, result: result);
        }
    }

    public abstract class TestResult<T> : ITestResult<T>
    {
        private readonly ITestTreeNode<T> _testTreeNode;
        private readonly bool _hasResult;
        private readonly object _result;
        private readonly Exception _arrangeException;
        private readonly Exception _actException;
        private readonly Exception _assertException;

        protected TestResult(ITestTreeNode<T> testTreeNode,
            bool hasResult,
            object result = null,
            Exception arrangeException = null,
            Exception actException = null,
            Exception assertException = null
            )
        {
            if (testTreeNode == null) throw new ArgumentNullException("testTreeNode");
            _testTreeNode = testTreeNode;
            _hasResult = hasResult;
            _result = result;
            _arrangeException = arrangeException;
            _actException = actException;
            _assertException = assertException;
        }

        public object Result { get { return HasResult ? _result : null; } }
        private Exception ArrangeException { get { return _arrangeException; } }
        private Exception ActException { get { return _actException; } }
        private Exception AssertException { get { return _assertException; } }

        public ITestTreeNode<T> TreeNode { get { return _testTreeNode; } }

        public bool IsSuccess { get { return ArrangeException == null && ActException == null && AssertException == null; } }
        private bool ArrangeFailed { get { return ArrangeException != null; } }
        private bool ActFailed { get { return ActException != null; } }
        private bool AssertFailed { get { return AssertException != null; } }

        internal string FailureMessage
        {
            get
            {
                if (ArrangeFailed) return string.Format("{0} thrown during Arrange : {1}", ArrangeException.GetType().Name, ArrangeException);
                if (ActFailed) return string.Format("{0} thrown during Act : {1}", ActException.GetType().Name, ActException);
                if (AssertFailed)
                {
                    // differentiate between Assertion error and other exception
                    //if (AssertException is AssertFailedException)
                    //{
                    //    return string.Format(AssertException.Message);
                    //}
                    return string.Format("{0} thrown during Assert : {1}", AssertException.GetType().Name, AssertException);
                }
                return null;
            }
        }

        public bool HasResult
        {
            get { return _hasResult; }
        }

        public string ResultMessage
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (HasResult)
                {
                    // ReSharper disable CompareNonConstrainedGenericWithNull
                    sb.AppendFormat("returned {0}", (Result != null) ? Result : (object)"<NULL>");
                    // ReSharper restore CompareNonConstrainedGenericWithNull
                    sb.Append(" - ");
                }
                if (IsSuccess)
                {
                    sb.Append("OK");
                }
                else
                {
                    sb.AppendFormat("KO\n\t{0}", FailureMessage);
                }

                return sb.ToString();
            }
        }

        public override string ToString()
        {
            return string.Format("{0} {1} -> {2}", IsSuccess ? "Passed" : "Failed", TreeNode, ResultMessage);
        }
    }

    internal class TestResult<T, TResult> : TestResult<T>, ITestResult<T, TResult>
    {
        private readonly TResult _result;

        internal TestResult(ITestTreeNode<T> testTreeNode,
            bool hasResult,
            TResult result = default(TResult),
            Exception arrangeException = null,
            Exception actException = null,
            Exception assertException = null
            )
            : base(testTreeNode, hasResult, result, arrangeException, actException, assertException)
        {
            _result = result;
        }

        TResult ITestResult<T, TResult>.Result { get { return HasResult ? _result : default(TResult); } }
    }

    internal class TestCompositeFluentNestedWrapper<T> : TestCompositeFluentWrapper<T>, ITestCompositeNestedFluent<T>
    {
        public TestCompositeFluentNestedWrapper(ITestComposite<T> wrapped)
            : base(wrapped)
        {
        }

        public ITestCompositeNestedFluent<T> IgnoreSelf(string reason = null)
        {
            this.IncludeSelfInEnumeration = false;
            return this;
        }

        //public ITestCompositeNestedFluent<T> IncludeSelf(string reason = null)
        //{
        //    this.IncludeSelfInEnumeration = true;
        //    return this;
        //}
    }
}
