using System.Linq.Expressions;
using System.Reflection;

namespace Shared.TestUtilities
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
					current = InstantiationHelpers.GetInstantiatedInstance(type);
					prop.SetValue(parent, current);
				}
			}

			currentMember = members.Pop();
			var finalProp = (PropertyInfo)currentMember;

			return (current, finalProp);
		}

		public static Expression<Func<TEntity, TProp>> ConvertExpression<TEntity, TInterface, TProp>(
			Expression<Func<TInterface, TProp>> expr)
		{
			if (expr.Body is not MemberExpression memberExpr)
				throw new ArgumentException("Expression body must be a member expression", nameof(expr));

			if (!typeof(TInterface).IsInterface) throw new ArgumentException($"{typeof(TInterface)} must be an interface type");
			if (!typeof(TEntity).IsAssignableTo(typeof(TInterface))) throw new ArgumentException($"{typeof(TInterface)} must be assignable from TEntity");

			var param = Expression.Parameter(typeof(TEntity), expr.Parameters[0].Name);

			Expression Rewrite(MemberExpression me)
			{
				if (me.Expression is ParameterExpression pe && pe == expr.Parameters[0])
				{
					var converted = Expression.Convert(param, typeof(TInterface));
					return Expression.MakeMemberAccess(converted, me.Member);
				}
				else if (me.Expression is MemberExpression inner)
				{
					var innerExpr = Rewrite(inner);
					return Expression.MakeMemberAccess(innerExpr, me.Member);
				}
				else
				{
					throw new InvalidOperationException($"Unexpected expression node: {me.Expression?.NodeType}");
				}
			}

			var newBody = Rewrite(memberExpr);
			return Expression.Lambda<Func<TEntity, TProp>>(newBody, param);
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
