using System.Reflection;
using System.Runtime.CompilerServices;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Creation.AutoConstructingProviders
{
    /// <summary>
    /// Provides automatic construction of object instances by resolving constructor parameters
    /// through a fixture context. Implements <see cref="IAutoConstructingProvider"/> to serve as
    /// the final fallback in the fixture resolution pipeline.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The provider selects the simplest (fewest parameters) constructor available on the requested
    /// type, preferring public constructors over non-public ones unless <see cref="FixtureOptions.PreferSimplestConstructor"/>
    /// is enabled. Constructors with a parameter whose type, after nullable unwrapping and type linking, is the
    /// requested type are tried last, and skipped unless <see cref="IFixtureContext.ValueProvider"/> resolves that parameter.
    /// Each constructor parameter is resolved recursively via <see cref="IFixtureContext.ValueProvider"/>,
    /// and parameters already resolved while choosing the constructor are not resolved again.
    /// </para>
    /// <para>
    /// The following types cannot be auto-constructed and will throw an <see cref="InvalidOperationException"/>:
    /// </para>
    /// <list type="bullet">
    ///   <item><description>Interfaces</description></item>
    ///   <item><description>Abstract classes</description></item>
    ///   <item><description>Delegates</description></item>
    ///   <item><description>Types with no discoverable constructors, except value types that declare none, which get a zeroed instance</description></item>
    /// </list>
    /// </remarks>
    internal class AutoConstructingProvider : IAutoConstructingProvider
    {
        public object AutoResolve(FixtureRequest request, IFixtureContext context, RecursiveResolveContext? recursiveResolveContext = null)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(context);

            if (request.Type.IsInterface || request.Type.IsAbstract)
                throw new InvalidOperationException($"Cannot construct types that are interfaces or abstract {request.Type.Name}.");

            if (request.Type.IsSubclassOf(typeof(MulticastDelegate)))
                throw new InvalidOperationException($"Cannot auto construct delegates: {request.Type.Name}");

            recursiveResolveContext ??= new();
            if (!recursiveResolveContext.Add(request.Type))
            {
                if (!context.OptionsFor(request.RootType).AllowResolveCircularDependencies)
                    throw new InvalidOperationException($"Circular dependency detected for {request.Type.Name} in UseAutoConstructor.");

                return recursiveResolveContext.AddShell(request.Type);
            }

            var selectedConstructor = GetConstructor(request, context);
            if (selectedConstructor is null)
            {
                if (request.Type.IsValueType
                    && request.Type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                        .Length == 0)
                {
                    return RuntimeHelpers.GetUninitializedObject(request.Type);
                }
                throw new InvalidOperationException($"Failed to find a constructor for {request.Type.Name}.");
            }

            var constructor = selectedConstructor.Constructor;

            var parameters = constructor.GetParameters();
            var parameterValues = GetParameterValues(parameters, request.RootType, selectedConstructor.ResolvedValues, context, recursiveResolveContext);

            try
            {
                var instance = constructor.Invoke([.. parameterValues]);
                recursiveResolveContext.Copy(request.Type, instance);
                return instance;
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                throw new InvalidOperationException($"Failed to AutoResolve {request.Type.Name}, see inner exception for details", ex);
            }
        }

        private IEnumerable<object?> GetParameterValues(ParameterInfo[] parameters, Type rootType, IReadOnlyDictionary<int, object?> resolvedValues, IFixtureContext context, RecursiveResolveContext recursiveResolveContext)
        {
            return parameters.Select(p =>
            {
                if (resolvedValues.TryGetValue(p.Position, out var value)) return value;

                var linked = context.UnwrapAndLink(p.ParameterType, rootType);
                var request = new FixtureRequest(linked, p, rootType);

                var result = context.ValueProvider.ResolveValue(request, context);
                if (result is not NoResult) return result;

                return AutoResolve(request, context, recursiveResolveContext.Branch());
            });
        }

        private static SelectedConstructor? GetConstructor(FixtureRequest request, IFixtureContext context)
        {
            var options = context.OptionsFor(request.RootType);
            SelectedConstructor? ctor;

            if (options.PreferSimplestConstructor && options.AllowPrivateConstructors)
            {
                ctor = GetSimplestConstructor(request, context, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            }
            else
            {
                ctor = GetSimplestConstructor(request, context, BindingFlags.Instance | BindingFlags.Public);

                if (ctor == null && options.AllowPrivateConstructors)
                    ctor = GetSimplestConstructor(request, context, BindingFlags.Instance | BindingFlags.NonPublic);
            }

            return ctor;
        }

        private static SelectedConstructor? GetSimplestConstructor(FixtureRequest request, IFixtureContext context, BindingFlags bindingAttr)
        {
            Dictionary<int, object?> resolvedValues = [];

            var constructor = request.Type
                .GetConstructors(bindingAttr)
                .OrderBy(c => c.GetParameters().Any(p => context.UnwrapAndLink(p.ParameterType, request.RootType) == request.Type))
                .ThenBy(c => c.GetParameters().Length)
                .FirstOrDefault(c => TryResolveSelfParameters(c, request, context, out resolvedValues));

            return constructor is null ? null : new SelectedConstructor(constructor, resolvedValues);
        }

        private static bool TryResolveSelfParameters(ConstructorInfo constructor, FixtureRequest request, IFixtureContext context, out Dictionary<int, object?> resolvedValues)
        {
            resolvedValues = [];

            foreach (var parameter in constructor.GetParameters())
            {
                var linked = context.UnwrapAndLink(parameter.ParameterType, request.RootType);
                if (linked != request.Type) continue;

                var parameterRequest = new FixtureRequest(linked, parameter, request.RootType);
                var value = context.ValueProvider.ResolveValue(parameterRequest, context);

                if (value is NoResult) return false;
                resolvedValues[parameter.Position] = value;
            }

            return true;
        }

        private record SelectedConstructor(ConstructorInfo Constructor, IReadOnlyDictionary<int, object?> ResolvedValues);
    }
}
