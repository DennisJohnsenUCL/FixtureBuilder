using System.Linq.Expressions;
using System.Reflection;

namespace FixtureBuilder.Helpers
{
    internal static class ExpressionHelpers
    {
        public static (object instance, PropertyInfo property) ResolvePropertyPath<TEntity, TProp>(TEntity root, Expression<Func<TEntity, TProp>> expr, bool instantiateTarget)
        {
            var memberExpr = expr.Body as MemberExpression
                ?? throw new ArgumentException($"Argument '{expr}' is invalid. Please use a direct property access, e.g., x => x.Property1.Property2, not a method, field, or computed value.", nameof(expr));

            if (root == null) throw new ArgumentException("Root must be initialized.");

            var members = new Stack<PropertyInfo>();
            while (memberExpr != null)
            {
                members.Push((PropertyInfo)memberExpr.Member);
                memberExpr = memberExpr.Expression as MemberExpression;
            }

            object current = root;

            while (members.Count > 1)
            {
                var prop = members.Pop();

                current = InitializePropertyValue(current, prop);
            }

            var finalProp = members.Pop();

            if (instantiateTarget) current = InitializePropertyValue(current, finalProp);

            return (current, finalProp);
        }

        private static object InitializePropertyValue(object parent, PropertyInfo prop)
        {
            var current = prop.GetValue(parent)!;

            if (current == null)
            {
                var type = prop.PropertyType;
                current = InstantiationHelpers.GetInstantiatedInstance(type, instantiateMembers: false);
                prop.SetValue(parent, current);
            }

            return current;
        }

        public static bool IsPropertyWritable<TTarget, TProp>(Expression<Func<TTarget, TProp>> expr)
        {
            if (expr.Body is not MemberExpression memberExpr)
                throw new ArgumentException("Expression must be a property access", nameof(expr));

            if (memberExpr.Member is not PropertyInfo propInfo)
                throw new ArgumentException("Expression must be a property", nameof(expr));

            return propInfo.CanWrite;
        }
    }
}
