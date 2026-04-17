using System.Reflection;
using FixtureBuilder.Core;

namespace FixtureBuilder.Creation.ConstructingProviders
{
    /// <summary>
    /// Creates instances of requested types using reflection-based constructor invocation.
    /// Supports both public and non-public constructors.
    /// </summary>
    internal class ConstructingProvider : IConstructingProvider
    {
        /// <summary>
        /// Creates an instance of the type specified in <paramref name="request"/> by invoking
        /// its constructor with the supplied arguments.
        /// </summary>
        /// <param name="request">The fixture request containing the <see cref="Type"/> to instantiate.</param>
        /// <param name="args">Constructor arguments, matched by position and type.</param>
        /// <returns>A new instance of the requested type.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the type cannot be instantiated with the given parameters — for example,
        /// if no matching constructor exists, the type is abstract, or a constructor argument is invalid.
        /// </exception>
        public object ResolveWithArguments(FixtureRequest request, params object?[] args)
        {
            if (request.Type.IsInterface || request.Type.IsAbstract)
                throw new InvalidOperationException($"Cannot construct types that are interfaces or abstract {type.Name}.");

            try
            {
                return Activator.CreateInstance(
                    type: request.Type,
                    bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                    binder: null,
                    args: args,
                    culture: null)!;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create {request.Type.Name} with given parameters.", ex);
            }
        }
    }
}
