using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Configuration
{
    internal class ExpressionResolver
    {
        private readonly Type _rootType;
        private readonly IFixtureContext _context;

        public ExpressionResolver(Type rootType, IFixtureContext context)
        {
            ArgumentNullException.ThrowIfNull(rootType);
            ArgumentNullException.ThrowIfNull(context);
            _rootType = rootType;
            _context = context;
        }

        private record PathStep(object Parent, DataMemberInfo DataMember, object Value);

        /// <summary>
        /// Walks a property access chain from the root object, initializing any <see langword="null"/>
        /// intermediate properties along the path, and passes the penultimate object together with
        /// the final member to <paramref name="configure"/>. Structs along the path are copies, so they
        /// are written back once <paramref name="configure"/> returns.
        /// </summary>
        /// <typeparam name="T">The type of the root object.</typeparam>
        /// <typeparam name="TProp">The type of the final property in the chain.</typeparam>
        /// <param name="root">The root object to begin traversal from.</param>
        /// <param name="expr">
        /// A property access chain expression, e.g., <c>x => x.Child.Grandchild.Tag</c>.
        /// It must already be validated with <see cref="ExpressionValidator"/>, as it is not checked here.
        /// </param>
        /// <param name="configure">
        /// Applies the change to the object that owns the final member. It must not resolve the same
        /// path again, as the write-back afterwards would overwrite those changes.
        /// </param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="root"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a property along the path does not have a getter, when a <see langword="null"/> member
        /// along the path cannot be initialized because nested instantiation is disabled, the property does not
        /// have a setter, or a provider returned <see langword="null"/>, or when a changed struct along the path
        /// is held by a property without a setter.
        /// </exception>
        public void ResolveDataMemberParent<T, TProp>(T root, Expression<Func<T, TProp>> expr, Action<object, DataMemberInfo> configure)
        {
            if (root == null) throw new ArgumentException("Root must be initialized.");

            var memberExpr = (MemberExpression)expr.Body;

            ResolveDataMemberPath(memberExpr, root, resolveInstance: false, configure);
        }

        /// <summary>
        /// Walks a property access chain from the root object, initializing any <see langword="null"/>
        /// properties along the path, including the final one, and passes the final property's value
        /// to <paramref name="configure"/>. Structs along the path, including the final value, are copies,
        /// so they are written back once <paramref name="configure"/> returns.
        /// </summary>
        /// <typeparam name="T">The type of the root object.</typeparam>
        /// <typeparam name="TProp">The type of the final property in the chain.</typeparam>
        /// <param name="root">The root object to begin traversal from.</param>
        /// <param name="expr">
        /// A property access chain expression, e.g., <c>x => x.Child.Grandchild.Tag</c>.
        /// It must already be validated with <see cref="ExpressionValidator"/>, as it is not checked here.
        /// </param>
        /// <param name="configure">
        /// Applies the change to the value of the final property, initialized if it was <see langword="null"/>.
        /// It must not resolve the same path again, as the write-back afterwards would overwrite those changes.
        /// </param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="root"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a property along the path does not have a getter, when a <see langword="null"/> member
        /// along the path cannot be initialized because nested instantiation is disabled, the property does not
        /// have a setter, or a provider returned <see langword="null"/>, or when a changed struct along the path
        /// is held by a property without a setter.
        /// </exception>
        public void ResolveDataMemberInstance<T, TProp>(T root, Expression<Func<T, TProp>> expr, Action<object, DataMemberInfo> configure)
        {
            if (root == null) throw new ArgumentException("Root must be initialized.");

            var memberExpr = (MemberExpression)expr.Body;

            ResolveDataMemberPath(memberExpr, root, resolveInstance: true, configure);
        }

        /// <summary>
        /// Walks a property access chain from the root object, initializing any <see langword="null"/>
        /// intermediate properties along the path. Structs along the path are written back, so members
        /// initialized inside them are kept.
        /// </summary>
        /// <typeparam name="T">The type of the root object.</typeparam>
        /// <param name="root">The root object to begin traversal from.</param>
        /// <param name="expr">
        /// A property access chain expression ending in a method invocation, e.g., <c>x => x.Child.Grandchild.Method()</c>.
        /// It must already be validated with <see cref="ExpressionValidator"/>, as it is not checked here.
        /// </param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="root"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a property along the path does not have a getter, when a <see langword="null"/> member
        /// along the path cannot be initialized because nested instantiation is disabled, the property does not
        /// have a setter, or a provider returned <see langword="null"/>, or when a changed struct along the path
        /// is held by a property without a setter.
        /// </exception>
        public void ResolveMethodParent<T>(T root, Expression<Action<T>> expr)
        {
            if (root == null) throw new ArgumentException("Root must be initialized.");

            var call = (MethodCallExpression)expr.Body;

            if (call.Object is MemberExpression memberExpr)
                ResolveDataMemberPath(memberExpr, root, resolveInstance: true, (_, _) => { });
        }

        /// <summary>
        /// Walks a chain of property accesses from a <see cref="MemberExpression"/>, initializing
        /// each intermediate property along the path. Passes the resolved instance and the final property
        /// to <paramref name="configure"/>, then writes structs along the path back into their parents.
        /// </summary>
        /// <param name="memberExpr">The member expression representing the property access chain.</param>
        /// <param name="root">The root object to begin resolution from.</param>
        /// <param name="resolveInstance">
        /// If <c>true</c>, the final property is also initialized and the instance passed to
        /// <paramref name="configure"/> is its value.
        /// If <c>false</c>, the instance passed to <paramref name="configure"/> is the parent that owns the final property.
        /// </param>
        /// <param name="configure">Applies the change to the resolved instance.</param>
        internal void ResolveDataMemberPath(MemberExpression? memberExpr, object root, bool resolveInstance, Action<object, DataMemberInfo> configure)
        {
            var members = new Stack<DataMemberInfo>();
            while (memberExpr != null)
            {
                if (!IsNullableValueProperty(memberExpr.Member))
                    members.Push(DataMemberInfo.FromMemberInfo(memberExpr.Member));
                memberExpr = memberExpr.Expression as MemberExpression;
            }

            object current = root;
            var path = new Stack<PathStep>();
            while (members.Count > 1)
            {
                var dataMember = members.Pop();
                var value = InitializeDataMemberValue(current, dataMember);
                path.Push(new PathStep(current, dataMember, value));
                current = value;
            }

            var finalDataMember = members.Pop();
            if (resolveInstance)
            {
                var value = InitializeDataMemberValue(current, finalDataMember);
                path.Push(new PathStep(current, finalDataMember, value));
                current = value;
            }

            configure(current, finalDataMember);
            WriteBackValueTypes(path);
        }

        /// <summary>
        /// Retrieves the current value of a property, or initializes it if <see langword="null"/>.
        /// When initialization is required, a new instance is resolved via the fixture context
        /// and assigned back to the property.
        /// </summary>
        /// <param name="parent">The object instance that owns the property.</param>
        /// <param name="dataMember">The property to read or initialize.</param>
        /// <returns>The existing or newly initialized value of the property.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the property does not have a getter, or when the value is <see langword="null"/> and
        /// nested instantiation is disabled, the property does not have a setter, or a provider returned <see langword="null"/>.
        /// </exception>
        internal object InitializeDataMemberValue(object parent, DataMemberInfo dataMember)
        {
            if (dataMember.TryIsPropertyInfo(out var pi) && !pi.CanRead)
                throw new InvalidOperationException($"Property {dataMember.Name} does not have a getter. It is not possible to work with nested properties unless every member in the chain has a getter.");

            var current = dataMember.GetValue(parent);
            if (current != null) return current;

            if (!_context.OptionsFor(_rootType).AllowInstantiateNestedMembers)
                throw new InvalidOperationException($"Property or field {dataMember.Name} in member chain is null, and instantiation of null chain members is disabled.");

            if (dataMember.IsPropertyInfo && !pi.CanWrite)
                throw new InvalidOperationException($"Property {dataMember.Name} does not have a setter. Please provide a value manually or with 'WithBackingField'");

            MemberInfo source = dataMember.IsPropertyInfo ? dataMember.Property : dataMember.Field;
            var request = new FixtureRequest(dataMember.DataMemberType, source, _rootType, dataMember.Name);

            current = _context.ProvideWithStrategy(request, _context.OptionsFor(_rootType).NestedMemberInstantiationMethod, InitializeMembers.None)
                ?? throw new InvalidOperationException($"User-registered Provider returned null for {dataMember.Name} in Expression chain resolution. " +
                $"Providers for types in Expression chain Resolution must return concrete values. " +
                $"Use Instantiate to explicitly instantiate the member instead.");

            dataMember.SetValue(parent, current);
            return current;
        }

        private static bool IsNullableValueProperty(MemberInfo member)
        {
            return member.Name == nameof(Nullable<>.Value)
                && Nullable.GetUnderlyingType(member.DeclaringType!) is not null;
        }

        /// <summary>
        /// Writes each struct value along the path back into its parent, innermost first, so changes
        /// made to the boxed copies reach the root.
        /// </summary>
        /// <param name="path">The members walked, with the parent each was read from and its value, last walked on top.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when a struct was changed but is held by a property without a setter.
        /// </exception>
        private static void WriteBackValueTypes(Stack<PathStep> path)
        {
            foreach (var (parent, dataMember, value) in path)
            {
                if (!dataMember.DataMemberType.IsValueType) continue;

                if (dataMember.TryIsPropertyInfo(out var pi) && !pi.CanWrite)
                {
                    if (RuntimeHelpers.Equals(pi.GetValue(parent), value)) continue;
                    throw new InvalidOperationException($"Property {dataMember.Name} holds a struct but does not have a setter, so changes to it cannot be written back. Please set the struct as a whole with 'WithBackingField' instead.");
                }

                dataMember.SetValue(parent, value);
            }
        }
    }
}
