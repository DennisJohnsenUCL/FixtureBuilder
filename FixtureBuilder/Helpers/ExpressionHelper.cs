using System.Linq.Expressions;
using System.Reflection;
using FixtureBuilder.FixtureContexts;
using FixtureBuilder.UninitializedProviders;

namespace FixtureBuilder.Helpers
{
    /// <summary>
    /// Provides utilities for validating, inspecting, and traversing lambda expressions
    /// that represent property access chains. Used to resolve property paths at runtime,
    /// initialize intermediate objects along a chain, and verify that targeted properties
    /// are writable.
    /// </summary>
    internal static class ExpressionHelper
    {
        /// <summary>
        /// Walks a property access chain from the root object, initializing any <see langword="null"/>
        /// intermediate properties along the path, and returns the penultimate object together with
        /// the final <see cref="PropertyInfo"/>.
        /// </summary>
        /// <typeparam name="T">The type of the root object.</typeparam>
        /// <typeparam name="TProp">The type of the final property in the chain.</typeparam>
        /// <param name="root">The root object to begin traversal from.</param>
        /// <param name="expr">
        /// A property access chain expression, e.g., <c>x => x.Child.Grandchild.Tag</c>.
        /// </param>
        /// <param name="context">The fixture context used to resolve new instances for <see langword="null"/> properties.</param>
        /// <returns>
        /// A tuple containing the object that owns the final property, and the <see cref="PropertyInfo"/>
        /// of that final property.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="root"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when <paramref name="expr"/> is not a valid property access chain, or when a
        /// <see langword="null"/> property along the path does not have a setter.
        /// </exception>
        public static (object Instance, DataMemberInfo DataMember) ResolveDataMemberParent<T, TProp>(T root, Expression<Func<T, TProp>> expr, IFixtureContext context)
        {
            if (root == null) throw new ArgumentException("Root must be initialized.");

            var memberExpr = (MemberExpression)expr.Body;

            return ResolveDataMemberPath(memberExpr, root, resolveInstance: false, context);
        }

        /// <summary>
        /// Walks a property access chain from the root object, initializing any <see langword="null"/>
        /// intermediate properties along the path, and returns the last object together with
        /// the final <see cref="PropertyInfo"/>.
        /// </summary>
        /// <typeparam name="T">The type of the root object.</typeparam>
        /// <typeparam name="TProp">The type of the final property in the chain.</typeparam>
        /// <param name="root">The root object to begin traversal from.</param>
        /// <param name="expr">
        /// A property access chain expression, e.g., <c>x => x.Child.Grandchild.Tag</c>.
        /// </param>
        /// <param name="context">The fixture context used to resolve new instances for <see langword="null"/> properties.</param>
        /// <returns>
        /// A tuple containing the object value of the final property, and the <see cref="PropertyInfo"/>
        /// of that final property.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="root"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when <paramref name="expr"/> is not a valid property access chain, or when a
        /// <see langword="null"/> property along the path does not have a setter.
        /// </exception>
        public static (object Instance, DataMemberInfo DataMember) ResolveDataMemberInstance<T, TProp>(T root, Expression<Func<T, TProp>> expr, IFixtureContext context)
        {
            if (root == null) throw new ArgumentException("Root must be initialized.");

            var memberExpr = (MemberExpression)expr.Body;

            return ResolveDataMemberPath(memberExpr, root, resolveInstance: true, context);
        }

        /// <summary>
        /// Walks a property access chain from the root object, initializing any <see langword="null"/>
        /// intermediate properties along the path.
        /// </summary>
        /// <typeparam name="T">The type of the root object.</typeparam>
        /// <param name="root">The root object to begin traversal from.</param>
        /// <param name="expr">
        /// A property access chain expression ending in a method invocation, e.g., <c>x => x.Child.Grandchild.Method()</c>.
        /// </param>
        /// <param name="context">The fixture context used to resolve new instances for <see langword="null"/> properties.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="root"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when <paramref name="expr"/> is not a valid property access chain, or when a
        /// <see langword="null"/> property along the path does not have a setter.
        /// </exception>
        public static void ResolveMethodParent<T>(T root, Expression<Action<T>> expr, IFixtureContext context)
        {
            if (root == null) throw new ArgumentException("Root must be initialized.");

            var call = (MethodCallExpression)expr.Body;

            if (call.Object is MemberExpression memberExpr)
                ResolveDataMemberPath(memberExpr, root, resolveInstance: true, context);
        }

        /// <summary>
        /// Walks a chain of property accesses from a <see cref="MemberExpression"/>, initializing
        /// each intermediate property along the path. Returns the final property and its owning instance.
        /// </summary>
        /// <param name="memberExpr">The member expression representing the property access chain.</param>
        /// <param name="root">The root object to begin resolution from.</param>
        /// <param name="resolveInstance">
        /// If <c>true</c>, the final property is also initialized and the returned instance is its value.
        /// If <c>false</c>, the returned instance is the parent that owns the final property.
        /// </param>
        /// <param name="context">The fixture context used to initialize property values.</param>
        /// <returns>A tuple of the resolved instance and the final property in the path.</returns>
        internal static (object Instance, DataMemberInfo DataMember) ResolveDataMemberPath(MemberExpression? memberExpr, object root, bool resolveInstance, IFixtureContext context)
        {
            var members = new Stack<DataMemberInfo>();
            while (memberExpr != null)
            {
                members.Push(DataMemberInfo.FromMemberInfo(memberExpr.Member));
                memberExpr = memberExpr.Expression as MemberExpression;
            }

            object current = root;
            while (members.Count > 1)
            {
                var dataMember = members.Pop();
                current = InitializeDataMemberValue(current, dataMember, context);
            }

            var finaldataMember = members.Pop();
            if (resolveInstance) current = InitializeDataMemberValue(current, finaldataMember, context);

            return (current, finaldataMember);
        }

        /// <summary>
        /// Retrieves the current value of a property, or initializes it if <see langword="null"/>.
        /// When initialization is required, a new instance is resolved via the fixture context
        /// and assigned back to the property.
        /// </summary>
        /// <param name="parent">The object instance that owns the property.</param>
        /// <param name="dataMember">The property to read or initialize.</param>
        /// <param name="context">The fixture context used to resolve a new instance when the property is <see langword="null"/>.</param>
        /// <returns>The existing or newly initialized value of the property.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the property value is <see langword="null"/> and the property does not have a setter.
        /// </exception>
        internal static object InitializeDataMemberValue(object parent, DataMemberInfo dataMember, IFixtureContext context)
        {
            if (dataMember.TryIsPropertyInfo(out var pi) && !pi.CanRead)
                throw new InvalidOperationException($"Property {dataMember.Name} does not have a getter. It is not possible to work with nested properties unless every member in the chain has a getter.");

            var current = dataMember.GetValue(parent);
            if (current != null) return current;

            if (!context.Options.AllowInstantiateNestedMembers)
                throw new InvalidOperationException($"Property or field {dataMember.Name} in member chain is null, and instantiation of null chain members is disabled.");

            if (dataMember.IsPropertyInfo && !pi.CanWrite)
                throw new InvalidOperationException($"Property {dataMember.Name} does not have a setter. Please provide a value manually or with 'WithBackingField'");

            var type = dataMember.DataMemberType;
            var request = dataMember.IsPropertyInfo
                ? new FixtureRequest(type, dataMember.Property, dataMember.Name)
                : new FixtureRequest(type, dataMember.Field, dataMember.Name);

            current = context.ProvideWithStrategy(request, context.Options.NestedMemberInstantiationMethod, InitializeMembers.None)
                ?? throw new InvalidOperationException($"User-registered Provider returned null for {type.Name} in Expression chain resolution. " +
                $"Providers for types in Expression chain Resolution must return concrete values. " +
                $"Use Instantiate to explicitly instantiate the member instead.");

            dataMember.SetValue(parent, current);
            return current;
        }

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
