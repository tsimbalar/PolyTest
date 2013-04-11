using System;
using System.Collections.Generic;

namespace PolyTest.Fluent
{
    public interface ITestCompositeFluent<T> : IFluentInterface, IEnumerable<ITestCase<T>>
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


        ITestExecutionReport<TResult> Walk<TResult>(Func<T, TResult> act, Action<TResult> assert);
    }
}