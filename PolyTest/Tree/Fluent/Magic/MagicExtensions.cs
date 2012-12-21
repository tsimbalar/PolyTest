using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace PolyTest.Tree.Fluent.Magic
{
    public static class MagicExtensions
    {

        public static ITestCompositeFluent<T> With<T, TProp>(this ITestCompositeFluent<T> root,
                                                          Expression<Func<T, TProp>> propertyAccessor,
            TProp newValue)
        {

            var mutation = MakeMutationWithReflection(propertyAccessor, newValue);
            return root.Consider(mutation);
        }

        public static ITestCompositeFluent<T> With<T, TProp>(this ITestCompositeFluent<T> root,
                                                                 Expression<Func<T, TProp>> propertyAccessor,
                                                                 TProp newValue,
                                                                 Func<ITestCompositeNestedFluent<T>,
                                                                     ITestCompositeFluent<T>> nestedAdd)
        {
            var mutation = MakeMutationWithReflection(propertyAccessor, newValue);
            return root.Consider(mutation, true, nestedAdd);
        }

        public static ITestCompositeFluent<T> WithNull<T, TProp>(this ITestCompositeFluent<T> root,
                                                                                Expression<Func<T, TProp>>
                                                                                    propertyAccessor) where TProp : class
        {
            var mutation = MakeMutationWithReflectionWithValueNull(propertyAccessor);
            return root.Consider(mutation);
        }


        public static ITestCompositeFluent<T> WithNullOrWhitespace<T>(this ITestCompositeFluent<T> root,
                                                                                Expression<Func<T, String>>
                                                                                    propertyAccessor)
        {
            var mutations = new List<IMutation<T>> {
                    MakeMutationWithReflectionWithValueNull(propertyAccessor),
                    MakeMutationWithReflectionWithStringValue(propertyAccessor, "", "empty string"),
                    MakeMutationWithReflectionWithStringValue(propertyAccessor, " ", "whitespace - space"),
                    MakeMutationWithReflectionWithStringValue(propertyAccessor, "\t", "whitespace - \\t"),
                };
            var result = root;
            foreach (var mutation in mutations)
            {
                result = result.Consider(mutation);
            }

            return result;
        }

        

        private static IMutation<T> MakeMutationWithReflection<T, TProp>(Expression<Func<T, TProp>> propertyAccessor,
                                                                         TProp newValue)
        {
            var propertyName = ReflectionHelper.GetFullPropertyName(propertyAccessor);
            string mutationDescription = string.Format("setting {0} = {1}", propertyName, newValue);
            var mutation = new Mutation<T>(mutationDescription, d => ReflectionHelper.Set(d, propertyAccessor, newValue));
            return mutation;
        }

        private static IMutation<T> MakeMutationWithReflection<T, TProp>(Expression<Func<T, TProp>> propertyAccessor,
                                                                         TProp newValue, string humanFriendlyDisplayValue)
        {
            var propertyName = ReflectionHelper.GetFullPropertyName(propertyAccessor);
            string mutationDescription = string.Format("setting {0} = {1}", propertyName, humanFriendlyDisplayValue);
            var mutation = new Mutation<T>(mutationDescription, d => ReflectionHelper.Set(d, propertyAccessor, newValue));
            return mutation;
        }

        private static IMutation<T> MakeMutationWithReflectionWithValueNull<T, TProp>(Expression<Func<T, TProp>> propertyAccessor) where TProp : class
        {
            return MakeMutationWithReflection(propertyAccessor, null, "<NULL>");
        }

        private static IMutation<T> MakeMutationWithReflectionWithStringValue<T>(Expression<Func<T, string>> propertyAccessor, string newValue, string valueDescription)
        {
            var propertyName = ReflectionHelper.GetFullPropertyName(propertyAccessor);
            string mutationDescription = string.Format("setting {0} = \"{1}\" ({2})", propertyName, newValue, valueDescription);
            var mutation = new Mutation<T>(mutationDescription, d => ReflectionHelper.Set(d, propertyAccessor, newValue));
            return mutation;
        }
    }
}
