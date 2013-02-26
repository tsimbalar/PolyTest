using System;
using System.Collections.Generic;

namespace PolyTest.Fluent
{
    public interface ITestCompositeFluent<T> : IFluentInterface
    {
        ITestCompositeFluent<T> Consider(IMutation<T> mutation);

        /// <summary>
        /// Consider this node, and a list of subnodes added as part of the <paramref name="nestedAdd"/> parameter.
        /// </summary>
        /// <param name="mutation"></param>
        /// <param name="nestedAdd"></param>
        /// <returns></returns>
        ITestCompositeFluent<T> Consider(IMutation<T> mutation,
                                         Func<ITestCompositeNestedFluent<T>, ITestCompositeFluent<T>> nestedAdd);


        void ForEach(Action<ITestCase<T>> action);
        IEnumerable<TResult> Select<TResult>(Func<ITestCase<T>, TResult> selector);
        ITestExecutionReport<T> Walk<TResult>(Func<T, TResult> act, Action<TResult> assert );

        IEnumerable<ITestCase<T>> AsEnumerable();
    }
}