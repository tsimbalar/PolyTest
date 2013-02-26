using System;
using System.Collections.Generic;
using System.Linq;
using PolyTest.Implementations;

namespace PolyTest.Fluent
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

        public ITestExecutionReport<T> Walk<TResult>(Func<T, TResult> act, Action<TResult> assert)
        {
            var report = new TestExecutionReport<T>();
            foreach (var testCase in this.AsEnumerablePrivate())
            {
                report.Add(testCase.Test(act, assert));
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