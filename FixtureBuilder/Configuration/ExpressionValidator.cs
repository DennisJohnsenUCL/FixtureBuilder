using System.Linq.Expressions;
using System.Reflection;

namespace FixtureBuilder.Configuration
{
    internal class ExpressionValidator
    {
        /// <summary>
        /// Validates that the final property in the specified expression has a setter.
        /// </summary>
        /// <typeparam name="T">The type of the entity the expression operates on.</typeparam>
        /// <typeparam name="TProp">The type of the property the expression returns.</typeparam>
        /// <param name="expr">
        /// A lambda expression targeting a property, e.g., <c>x => x.Name</c>.
        /// It must already be validated with <see cref="ValidatePropertyExpression{T, TProp}"/>, as it is not checked here.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the property targeted by <paramref name="expr"/> does not have a setter.
        /// </exception>
        public static void ValidatePropertyWriteable<T, TProp>(Expression<Func<T, TProp>> expr)
        {
            var property = (PropertyInfo)((MemberExpression)expr.Body).Member;
            if (!property.CanWrite)
                throw new InvalidOperationException($"{property.Name} Does not contain a setter. Please use With or WithField when wanting to set the value of a property without a setter.");
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
        /// Thrown when <paramref name="expr"/> is not a direct property or field access chain, which includes
        /// expressions that reference methods, constants, computed values, or the bare parameter itself, or when
        /// it ends in <see cref="Nullable{T}.Value"/> instead of the nullable member itself.
        /// </exception>
        public static void ValidateExpression<T, TProp>(Expression<Func<T, TProp>> expr)
        {
            if (expr.Body is not MemberExpression || !IsMemberChainOnParameter(expr.Body, expr.Parameters[0]))
                throw new InvalidOperationException(
                    "Expression must be a direct property or field access chain, e.g., x => x.Field1.Property2. Methods, constants, and computed values are not valid.");

            ThrowIfEndsInNullableValue(expr);
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
        /// Thrown when <paramref name="expr"/> is not a direct property or field access chain terminating in a property,
        /// which includes expressions that reference methods, constants, computed values, or the bare parameter itself,
        /// or when it ends in <see cref="Nullable{T}.Value"/> instead of the nullable member itself.
        /// </exception>
        public static void ValidatePropertyExpression<T, TProp>(Expression<Func<T, TProp>> expr)
        {
            if (expr.Body is not MemberExpression { Member: PropertyInfo } || !IsMemberChainOnParameter(expr.Body, expr.Parameters[0]))
                throw new InvalidOperationException(
                    "Expression must be a direct property or field access chain terminating in a property, e.g., x => x.Field1.Property2. Methods, constants, and computed values are not valid.");

            ThrowIfEndsInNullableValue(expr);
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
        /// Thrown when <paramref name="expr"/> is not an instance method call on the lambda parameter or on a direct
        /// property or field access chain rooted at it. This includes static and extension methods, and calls on
        /// constants, computed values, or the results of other method calls.
        /// </exception>
        public static void ValidateMethodExpression<T>(Expression<Action<T>> expr)
        {
            if (expr.Body is not MethodCallExpression { Object: { } obj } || !IsMemberChainOnParameter(obj, expr.Parameters[0]))
                throw new InvalidOperationException(
                    "Expression must be a method call on a direct property access chain, e.g., x => x.Property1.DoSomething(). Static methods, extension methods, constants, and calls not rooted in the parameter are not valid.");
        }

        private static void ThrowIfEndsInNullableValue(LambdaExpression expr)
        {
            if (expr.Body is MemberExpression me && IsNullableValueProperty(me.Member))
                throw new InvalidOperationException(
                    $"Expression '{expr}' ends in Nullable<T>.Value. The expression selects the member to configure, so target the nullable member itself: '{Expression.Lambda(me.Expression!, expr.Parameters)}'.");
        }

        internal static bool IsNullableValueProperty(MemberInfo member)
        {
            return member.Name == nameof(Nullable<>.Value)
                && Nullable.GetUnderlyingType(member.DeclaringType!) is not null;
        }

        internal static bool IsMemberChainOnParameter(Expression expression, ParameterExpression param)
        {
            Expression current = expression;
            while (current is MemberExpression member)
            {
                current = member.Expression!;
            }
            return current == param;
        }
    }
}
