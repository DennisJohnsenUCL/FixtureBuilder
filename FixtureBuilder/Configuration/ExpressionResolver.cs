using System.Linq.Expressions;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Creation.UninitializedProviders;

namespace FixtureBuilder.Configuration
{
    internal class ExpressionResolver
    {
        private readonly Type _rootType;

        public ExpressionResolver(Type rootType)
        {
            ArgumentNullException.ThrowIfNull(rootType);
            _rootType = rootType;
        }

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
        public (object Instance, DataMemberInfo DataMember) ResolveDataMemberParent<T, TProp>(T root, Expression<Func<T, TProp>> expr, IFixtureContext context)
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
        public (object Instance, DataMemberInfo DataMember) ResolveDataMemberInstance<T, TProp>(T root, Expression<Func<T, TProp>> expr, IFixtureContext context)
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
        public void ResolveMethodParent<T>(T root, Expression<Action<T>> expr, IFixtureContext context)
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
        internal (object Instance, DataMemberInfo DataMember) ResolveDataMemberPath(MemberExpression? memberExpr, object root, bool resolveInstance, IFixtureContext context)
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

            var finalDataMember = members.Pop();
            if (resolveInstance) current = InitializeDataMemberValue(current, finalDataMember, context);

            return (current, finalDataMember);
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
        internal object InitializeDataMemberValue(object parent, DataMemberInfo dataMember, IFixtureContext context)
        {
            if (dataMember.TryIsPropertyInfo(out var pi) && !pi.CanRead)
                throw new InvalidOperationException($"Property {dataMember.Name} does not have a getter. It is not possible to work with nested properties unless every member in the chain has a getter.");

            var current = dataMember.GetValue(parent);
            if (current != null) return current;

            if (!context.OptionsFor(_rootType).AllowInstantiateNestedMembers)
                throw new InvalidOperationException($"Property or field {dataMember.Name} in member chain is null, and instantiation of null chain members is disabled.");

            if (dataMember.IsPropertyInfo && !pi.CanWrite)
                throw new InvalidOperationException($"Property {dataMember.Name} does not have a setter. Please provide a value manually or with 'WithBackingField'");

            var type = dataMember.DataMemberType;
            var name = dataMember.Name;
            var request = dataMember.IsPropertyInfo
                ? new FixtureRequest(type, dataMember.Property, _rootType, name)
                : new FixtureRequest(type, dataMember.Field, _rootType, name);

            current = context.ProvideWithStrategy(request, context.OptionsFor(_rootType).NestedMemberInstantiationMethod, InitializeMembers.None)
                ?? throw new InvalidOperationException($"User-registered Provider returned null for {type.Name} in Expression chain resolution. " +
                $"Providers for types in Expression chain Resolution must return concrete values. " +
                $"Use Instantiate to explicitly instantiate the member instead.");

            dataMember.SetValue(parent, current);
            return current;
        }
    }
}
