using System.Linq.Expressions;
using System.Reflection;

namespace FixtureBuilder.Helpers
{
    internal static class ExpressionHelper
    {
        public static (object instance, PropertyInfo property) ResolvePropertyPath<TEntity, TProp>(TEntity root, Expression<Func<TEntity, TProp>> expr, bool instantiateTarget)
        {
            ValidateExpression(expr);

            var memberExpr = (MemberExpression)expr.Body;

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
            var current = prop.GetValue(parent);

            if (current == null)
            {
                if (!prop.CanWrite)
                    throw new InvalidOperationException($"Property {prop.Name} does not have a setter. Please provide a value manually or with 'WithBackingField'");

                var type = prop.PropertyType;
                current = InstantiationHelper.GetInstantiatedInstance(type, instantiateMembers: false);
                prop.SetValue(parent, current);
            }

            return current;
        }

        public static bool IsPropertyWritable<TEntity, TProp>(Expression<Func<TEntity, TProp>> expr)
        {
            if (expr.Body is MemberExpression me && me.Member is PropertyInfo pi)
                return pi.CanWrite;

            return false;
        }

        public static void ValidatePropertyWriteable<TEntity, TProp>(Expression<Func<TEntity, TProp>> expr)
        {
            if (!IsPropertyWritable(expr))
                throw new InvalidOperationException($"{typeof(TProp).Name} Does not contain a setter. Please use With or WithField when wanting to set the value of a property without a setter.");
        }

        public static void ValidateExpression<TEntity, TProp>(Expression<Func<TEntity, TProp>> expr)
        {
            var message = $"Expression must be a direct property access chain, e.g., x => x.Property1.Property2. Methods, constants, fields, and computed values are not valid.";

            var param = expr.Parameters[0];

            if (expr.Body == param) throw new InvalidOperationException(message);

            Expression current = expr.Body;
            while (current is MemberExpression { Member: PropertyInfo } member)
            {
                current = member.Expression!;
            }

            if (expr.Body is not MemberExpression || current != param)
                throw new InvalidOperationException(message);
        }
    }
}
