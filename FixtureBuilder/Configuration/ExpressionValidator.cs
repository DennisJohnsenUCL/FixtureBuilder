using System.Linq.Expressions;
using System.Reflection;

namespace FixtureBuilder.Configuration
{
    internal class ExpressionValidator
    {
        /// <summary>
        /// Determines whether the final property in the specified expression is writable.
        /// </summary>
        /// <typeparam name="T">The type of the entity the expression operates on.</typeparam>
        /// <typeparam name="TProp">The type of the property the expression returns.</typeparam>
        /// <param name="expr">A lambda expression targeting a property, e.g., <c>x => x.Name</c>.</param>
        /// <returns>
        /// <see langword="true"/> if the expression body is a property access and the property has a setter;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsPropertyWritable<T, TProp>(Expression<Func<T, TProp>> expr)
        {
            if (expr.Body is MemberExpression me && me.Member is PropertyInfo pi)
                return pi.CanWrite;

            return false;
        }

        /// <summary>
        /// Validates that the final property in the specified expression has a setter.
        /// </summary>
        /// <typeparam name="T">The type of the entity the expression operates on.</typeparam>
        /// <typeparam name="TProp">The type of the property the expression returns.</typeparam>
        /// <param name="expr">A lambda expression targeting a property, e.g., <c>x => x.Name</c>.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the property targeted by <paramref name="expr"/> does not have a setter.
        /// </exception>
        public static void ValidatePropertyWriteable<T, TProp>(Expression<Func<T, TProp>> expr)
        {
            if (!IsPropertyWritable(expr))
                throw new InvalidOperationException($"{typeof(TProp).Name} Does not contain a setter. Please use With or WithField when wanting to set the value of a property without a setter.");
        }

        /// <summary>
        /// Validates that the specified expression represents a direct property or field access chain
        /// rooted at the lambda parameter.
        /// </summary>
        /// <typeparam name="T">The type of the entity the expression operates on.</typeparam>
        /// <typeparam name="TProp">The type of the property the expression returns.</typeparam>
        /// <param name="expr">
        /// A lambda expression expected to be a property or field access chain, e.g., <c>x => x.Property1.Property2</c>.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when <paramref name="expr"/> is not a direct property or field access chain. This includes
        /// expressions that reference methods, constants, computed values, or the bare parameter itself.
        /// </exception>
        public static void ValidateExpression<T, TProp>(Expression<Func<T, TProp>> expr)
        {
            if (expr.Body is not MemberExpression || !TryWalkToParameter(expr.Body, expr.Parameters[0]))
                throw new InvalidOperationException(
                    "Expression must be a direct property or field access chain, e.g., x => x.Field1.Property2. Methods, constants, and computed values are not valid.");
        }

        /// <summary>
        /// Validates that the specified expression represents a direct property or field access chain terminating in a property
        /// rooted at the lambda parameter.
        /// </summary>
        /// <typeparam name="T">The type of the entity the expression operates on.</typeparam>
        /// <typeparam name="TProp">The type of the property the expression returns.</typeparam>
        /// <param name="expr">
        /// A lambda expression expected to be a property or field access chain, e.g., <c>x => x.Property1.Property2</c>.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when <paramref name="expr"/> is not a direct property or field access chain. This includes
        /// expressions that reference methods, constants, computed values, or the bare parameter itself.
        /// </exception>
        public static void ValidatePropertyExpression<T, TProp>(Expression<Func<T, TProp>> expr)
        {
            if (expr.Body is not MemberExpression { Member: PropertyInfo } || !TryWalkToParameter(expr.Body, expr.Parameters[0]))
                throw new InvalidOperationException(
                    "Expression must be a direct property or field access chain terminating in a property, e.g., x => x.Field1.Property2. Methods, constants, and computed values are not valid.");
        }

        /// <summary>
        /// Validates that the specified expression represents a direct property or field access chain
        /// rooted at the lambda parameter, and ending in a method invocation.
        /// </summary>
        /// <typeparam name="T">The type of the entity the expression operates on.</typeparam>
        /// <param name="expr">
        /// A lambda expression expected to be a property or field access chain ending in a method invocation, e.g., <c>x => x.Property1.Method()</c>.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when <paramref name="expr"/> is not a direct property or field access chain. This includes
        /// expressions that reference constants, fields, computed values, or the bare parameter itself.
        /// </exception>
        public static void ValidateMethodExpression<T>(Expression<Action<T>> expr)
        {
            if (expr.Body is not MethodCallExpression { Object: { } obj } || !TryWalkToParameter(obj, expr.Parameters[0]))
                throw new InvalidOperationException(
                    "Expression must be a method call on a direct property access chain, e.g., x => x.Property1.DoSomething(). Static methods, extension methods, constants, and calls not rooted in the parameter are not valid.");
        }

        internal static bool TryWalkToParameter(Expression body, ParameterExpression param)
        {
            Expression current = body;
            while (current is MemberExpression member)
            {
                current = member.Expression!;
            }
            return current == param;
        }
    }
}
