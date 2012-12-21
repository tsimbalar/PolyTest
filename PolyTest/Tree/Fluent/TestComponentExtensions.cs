using System;

namespace PolyTest.Tree.Fluent
{
    public static class TestComponentExtensions
    {
        public static ITestCompositeFluent<T> Consider<T>(this ITestCompositeFluent<T> tree,
                                                          string mutationDescr, Action<T> mutationAction)
        {
            return tree.Consider(new Mutation<T>(mutationDescr, mutationAction));
        }

        public static ITestCompositeFluent<T> Consider<T>(this ITestCompositeFluent<T> tree,
                                                          string mutationDescription, Action<T> mutationAction,
                                                          Func<ITestCompositeNestedFluent<T>, ITestCompositeFluent<T>> nestedAdd)
        {
            return tree.Consider(new Mutation<T>(mutationDescription, mutationAction), true, nestedAdd);
        }
    }
}