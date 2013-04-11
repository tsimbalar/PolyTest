using System;
using System.Collections.Generic;
using System.Linq;
using PolyTest.Fluent;

namespace PolyTest.Implementations.Fluent
{
    internal abstract class TestCompositeFluentWrapperBase<T> : ITestCompositeFluent<T>
    {
        private readonly ITestComposite<T> _wrapped;

        // visible for testing only
        internal ITestComposite<T> Wrapped { get { return _wrapped; } }

        protected TestCompositeFluentWrapperBase(ITestComposite<T> wrapped)
        {
            if (wrapped == null) throw new ArgumentNullException("wrapped");
            _wrapped = wrapped;
        }

        protected bool IncludeSelfInEnumeration
        {
            get { return _wrapped.IncludeSelfInEnumeration; }
            set { _wrapped.IncludeSelfInEnumeration = value; }
        }

        public ITestCompositeFluent<T> Consider(IMutation<T> mutation)
        {
            this._wrapped.Add(Poly.Create.Composite(this._wrapped, mutation));
            return this;
        }

        public ITestCompositeFluent<T> Consider(IMutation<T> mutation, Func<ITestCompositeNestedFluent<T>, ITestCompositeFluent<T>> nestedAdd)
        {
            ITestCompositeNestedFluent<T> composite = new TestCompositeFluentNestedWrapper<T>(new TestComposite<T>(this._wrapped, mutation));
            var updatedComposite = nestedAdd(composite);

            if (updatedComposite == null)
            {
                throw new InvalidOperationException("nestedAdd returned null");
            }
            var compositeWithNewNodes = updatedComposite as TestCompositeFluentWrapperBase<T>;
            if (compositeWithNewNodes == null)
            {
                throw new InvalidOperationException(string.Format("Expected nestedAdd to return an instance of type {0}. It returned : {1}", typeof(TestCompositeFluentWrapperBase<T>), updatedComposite.GetType()));
            }

            this._wrapped.Add(compositeWithNewNodes._wrapped);

            return this;
        }


        public IEnumerable<ITestCase<T>> Flatten()
        {
            return Wrapped.Enumerate();
        }

        public ITestExecutionReport<TResult> Walk<TResult>(Func<T, TResult> act, Action<TResult> assert)
        {
            var report = new TestExecutionReport<TResult>();
            foreach (var testCase in this.Flatten().Select((tc, i) => new { TestCase = tc, Index = i }))
            {
                var result = TestRunner.Run(testCase.Index, testCase.TestCase, act, assert);
                report.Add(result);
            }
            return report;
        }
    }
}