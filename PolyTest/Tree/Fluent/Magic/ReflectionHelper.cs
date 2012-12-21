using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace PolyTest.Tree.Fluent.Magic
{

    /// <summary>
    /// Utility methods to play with Reflection (usually findout the name of a property from a lambda etc)
    /// </summary>
    public static class ReflectionHelper
    {


        #region Get the full (dot separated) name of a property

        // from http://stackoverflow.com/a/2789606/474763

        /// <summary>
        /// Get the  fully qualified name of a property (Root.Nested.SomeProperty)
        /// </summary>
        public static string GetFullPropertyName<T, TProperty>(Expression<Func<T, TProperty>> exp)
        {
            MemberExpression memberExp;
            if (!TryFindMemberExpression(exp.Body, out memberExp)) return string.Empty;

            var memberNames = new Stack<string>();
            do
            {
                memberNames.Push(memberExp.Member.Name);
            }
            while (TryFindMemberExpression(memberExp.Expression, out memberExp));

            return string.Join(".", memberNames.ToArray());
        }

        private static bool TryFindMemberExpression(Expression exp, out MemberExpression memberExp)
        {
            memberExp = exp as MemberExpression;
            if (memberExp != null)
            {
                // heyo! that was easy enough
                return true;
            }

            // if the compiler created an automatic conversion,
            // it'll look something like...
            // obj => Convert(obj.Property) [e.g., int -> object]
            // OR:
            // obj => ConvertChecked(obj.Property) [e.g., int -> long]
            // ...which are the cases checked in IsConversion
            if (IsConversion(exp) && exp is UnaryExpression)
            {
                memberExp = ((UnaryExpression)exp).Operand as MemberExpression;
                if (memberExp != null)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsConversion(Expression exp)
        {
            return (
                       exp.NodeType == ExpressionType.Convert ||
                       exp.NodeType == ExpressionType.ConvertChecked
                   );
        }

        #endregion


        // borrowed from http://stackoverflow.com/questions/10939750/c-how-to-set-properties-and-nested-propertys-properties-with-expression
        // setting the value for a nested property 

        /// <summary>
        /// Sets the value of a property (possibly nested) of a root object
        /// For instance Set((Bazar) theBazar, b=> b.Bidule.Machin, 42)
        /// </summary>
        public static void Set<TRoot, TValue>(TRoot root, Expression<Func<TRoot, TValue>> func, TValue value)
        {
            MemberExpression mex = func.Body as MemberExpression;
            if (mex == null) throw new ArgumentException();

            var pi = mex.Member as PropertyInfo;
            if (pi == null) throw new ArgumentException();

            object target = GetTarget(root, mex.Expression);
            pi.SetValue(target, value, null);
        }


        // look for the target of a nested property accessor
        private static object GetTarget<TUnderTest>(TUnderTest root, Expression expr)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.Parameter:
                    return root; // reached the end of the tree ... apply to root
                case ExpressionType.MemberAccess:
                    MemberExpression mex = (MemberExpression)expr;
                    PropertyInfo pi = mex.Member as PropertyInfo;
                    if (pi == null) throw new ArgumentException();
                    object target = GetTarget(root, mex.Expression);
                    return pi.GetValue(target, null);
                default:
                    throw new InvalidOperationException();
            }
        }

    }
}