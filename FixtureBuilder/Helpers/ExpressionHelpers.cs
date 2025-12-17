using System.Linq.Expressions;
using System.Reflection;

namespace FixtureBuilder.Helpers
{
    internal static class ExpressionHelpers
    {
        public static (object instance, PropertyInfo property) ResolvePropertyPath<TEntity, TProp>(TEntity root, Expression<Func<TEntity, TProp>> expr)
        {
            var memberExpr = expr.Body as MemberExpression
                ?? throw new ArgumentException("Expression must be a property access", nameof(expr));

            if (root == null) throw new ArgumentException("Root must be initialized.");

            var members = new Stack<MemberInfo>();
            while (memberExpr != null)
            {
                members.Push(memberExpr.Member);
                memberExpr = memberExpr.Expression as MemberExpression;
            }

            object current = root;
            MemberInfo currentMember;

            while (members.Count > 1)
            {
                currentMember = members.Pop();
                var prop = (PropertyInfo)currentMember;

                var parent = current;
                current = prop.GetValue(parent)!;

                if (current == null)
                {
                    var type = prop.PropertyType;
                    current = InstantiationHelpers.GetInstantiatedInstance(type, instantiateMembers: false);
                    prop.SetValue(parent, current);
                }
            }

            currentMember = members.Pop();
            var finalProp = (PropertyInfo)currentMember;

            return (current, finalProp);
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
