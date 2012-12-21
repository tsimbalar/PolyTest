using System;
using System.Collections.Generic;

namespace PolyTest.Tree.Fluent
{
    public interface ITestCompositeFluent<T>
    {
        ITestCompositeFluent<T> Consider(IMutation<T> mutation);

        ITestCompositeFluent<T> Consider(IMutation<T> mutation,
                                         bool includeMutationInTestCase,
                                         Func<ITestCompositeNestedFluent<T>, ITestCompositeFluent<T>> nestedAdd);

        void Walk(Action<ITestTreeNode<T>> action);
        IEnumerable<TResult> Walk<TResult>(Func<ITestTreeNode<T>, TResult> tranformation);

        ITestExecutionReport<T> Walk<TResult>(Func<T, TResult> act, Action<TResult> assert );
    }
}