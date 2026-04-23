using System.Reflection;
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
    /// type, preferring public constructors over non-public ones. Each constructor parameter is
    /// resolved recursively via <see cref="IFixtureContext.ResolveParameterValue"/>.
    /// </para>
    /// <para>
    /// The following types cannot be auto-constructed and will throw an <see cref="InvalidOperationException"/>:
    /// </para>
    /// <list type="bullet">
    ///   <item><description>Interfaces</description></item>
    ///   <item><description>Abstract classes</description></item>
    ///   <item><description>Types with no discoverable constructors</description></item>
    /// </list>
    /// </remarks>
    internal class AutoConstructingProvider : IAutoConstructingProvider
    {
        public object AutoResolve(FixtureRequest request, IFixtureContext context, RecursiveResolveContext? recursiveResolveContext = null)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(context);

            recursiveResolveContext ??= new();
            if (!recursiveResolveContext.Add(request.Type))
            {
                if (!context.OptionsFor(request.RootType).AllowResolveCircularDependencies)
                    throw new InvalidOperationException($"Circular dependency detected for {request.Type.Name} in UseAutoConstructor.");

                return recursiveResolveContext.AddShell(request.Type);
            }

            if (request.Type.IsInterface || request.Type.IsAbstract)
                throw new InvalidOperationException($"Cannot construct types that are interfaces or abstract {request.Type.Name}.");

            var constructor = GetConstructorInfo(request, context);
            var parameters = constructor.GetParameters();
            var parameterValues = GetParameterValues(parameters, request.RootType, context, recursiveResolveContext);

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

        private IEnumerable<object?> GetParameterValues(ParameterInfo[] parameters, Type rootType, IFixtureContext context, RecursiveResolveContext recursiveResolveContext)
        {
            return parameters.Select(p =>
            {
                var paramType = context.UnwrapAndLink(p.ParameterType);

                var request = new FixtureRequest(paramType, p, rootType, p.Name);
                var result = context.ValueProvider.ResolveValue(request, context);
                if (result is not NoResult) return result;

                return AutoResolve(request, context, recursiveResolveContext.Branch());
            });
        }

        private static ConstructorInfo GetConstructorInfo(FixtureRequest request, IFixtureContext context)
        {
            var options = context.OptionsFor(request.RootType);
            ConstructorInfo? ctor;

            if (options.PreferSimplestConstructor && options.AllowPrivateConstructors)
            {
                ctor = GetSimplestConstructor(request.Type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            }
            else
            {
                ctor = GetSimplestConstructor(request.Type, BindingFlags.Instance | BindingFlags.Public);

                if (ctor == null && options.AllowPrivateConstructors)
                    ctor = GetSimplestConstructor(request.Type, BindingFlags.Instance | BindingFlags.NonPublic);
            }

            return ctor ?? throw new InvalidOperationException($"Failed to find a constructor for {request.Type.Name}.");
        }

        private static ConstructorInfo? GetSimplestConstructor(Type type, BindingFlags bindingAttr)
        {
            return type
                .GetConstructors(bindingAttr)
                .OrderBy(c => c.GetParameters().Length)
                .FirstOrDefault();
        }
    }
}
