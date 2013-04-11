using System;
using System.Collections;
using System.Collections.Generic;
using PolyTest.Fluent;

namespace PolyTest.Tests.TestUtils.Fluent
{
    internal class DummyTestCompositeFluent<T> : ITestCompositeFluent<ClassToTest>
    {
        public ITestCompositeFluent<ClassToTest> Consider(IMutation<ClassToTest> mutation)
        {
            throw new NotImplementedException();
        }

        public ITestCompositeFluent<ClassToTest> Consider(IMutation<ClassToTest> mutation, Func<ITestCompositeNestedFluent<ClassToTest>, ITestCompositeFluent<ClassToTest>> nestedAdd)
        {
            throw new NotImplementedException();
        }

        public void ForEach(Action<ITestCase<ClassToTest>> action)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TResult> Select<TResult>(Func<ITestCase<ClassToTest>, TResult> selector)
        {
            throw new NotImplementedException();
        }

        public ITestExecutionReport<TResult> Walk<TResult>(Func<ClassToTest, TResult> act, Action<TResult> assert)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ITestCase<ClassToTest>> Flatten()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<ITestCase<ClassToTest>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}