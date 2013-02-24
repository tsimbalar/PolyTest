using System;
using System.Collections.Generic;

namespace PolyTest.Tree.Fluent
{
    public interface ITestCompositeFluent<T>
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


        void Walk(Action<ITestTreeNode<T>> action);


        IEnumerable<TResult> Walk<TResult>(Func<ITestTreeNode<T>, TResult> tranformation);

        ITestExecutionReport<T> Walk<TResult>(Func<T, TResult> act, Action<TResult> assert );

        IEnumerable<ITestTreeNode<T>> AsEnumerable();
    }
}