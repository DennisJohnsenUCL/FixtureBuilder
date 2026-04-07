using System.Reflection;
using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.AutoConstructingProviders
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
        public object AutoResolve(FixtureRequest request, IFixtureContext context)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(context);

            if (request.Type.IsInterface || request.Type.IsAbstract)
                throw new InvalidOperationException($"Cannot construct types that are interfaces or abstract {request.Type.Name}.");

            var constructor = GetConstructorInfo(request, context);
            var parameters = constructor.GetParameters();
            var parameterValues = parameters.Select(p => context.ResolveParameterValue(p, context));

            try
            {
                var instance = constructor.Invoke([.. parameterValues]);
                return instance;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to AutoResolve {request.Type.Name}, see inner exception for details", ex);
            }
        }

        private static ConstructorInfo GetConstructorInfo(FixtureRequest request, IFixtureContext context)
        {
            var constructor = GetSimplestConstructor(request.Type, BindingFlags.Public | BindingFlags.Instance);

            if (constructor == null && context.Options.AllowPrivateConstructors)
            {
                constructor ??= GetSimplestConstructor(request.Type, BindingFlags.NonPublic | BindingFlags.Instance);
            }

            if (constructor == null)
                throw new InvalidOperationException($"Failed to find a constructor for {request.Type.Name}.");

            return constructor;
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
