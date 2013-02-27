using System;

namespace PolyTest.Fluent
{
    public static class TestCompositeFluentExtensions
    {
        public static ITestCompositeFluent<T> Consider<T>(this ITestCompositeFluent<T> parent,
                                                          string mutationDescr, Action<T> mutationAction)
        {
            return parent.Consider(Poly.Factory.Mutation(mutationDescr, mutationAction));
        }

        public static ITestCompositeFluent<T> Consider<T>(this ITestCompositeFluent<T> parent,
                                                          string mutationDescription, Action<T> mutationAction,
                                                          Func<ITestCompositeNestedFluent<T>, ITestCompositeFluent<T>> nestedAdd)
        {
            return parent.Consider(Poly.Factory.Mutation(mutationDescription, mutationAction), nestedAdd);
        }
    }
}