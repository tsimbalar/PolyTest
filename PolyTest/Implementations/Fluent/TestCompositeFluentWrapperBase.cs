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

            this._wrapped.Add(((TestCompositeFluentWrapperBase<T>)updatedComposite)._wrapped);

            return this;
        }

        public void ForEach(Action<ITestCase<T>> action)
        {
            foreach (var testTreeNode in this.AsEnumerablePrivate())
            {
                action(testTreeNode);
            }
        }

        public IEnumerable<TResult> Select<TResult>(Func<ITestCase<T>, TResult> selector)
        {
            return this.AsEnumerablePrivate().Select(selector);
        }

        public ITestExecutionReport<TResult> Walk<TResult>(Func<T, TResult> act, Action<TResult> assert)
        {
            var report = new TestExecutionReport<TResult>();
            foreach (var testCase in this.AsEnumerablePrivate())
            {
                var result = testCase.Test(act, assert);
                report.Add(result);
            }
            return report;
        }

        public IEnumerable<ITestCase<T>> AsEnumerable()
        {
            return AsEnumerablePrivate();
        }

        private IEnumerable<TestCaseItem<T>> AsEnumerablePrivate()
        {
            var resultEnumerable = _wrapped.Enumerate().Select((tc, i) => new TestCaseItem<T>(i, tc));
            return resultEnumerable;
        }

    }
}