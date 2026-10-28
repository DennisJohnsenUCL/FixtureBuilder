using System.Reflection;

namespace FixtureBuilder.Creation.ConstructingProviders
{
    /// <summary>
    /// Creates instances of requested types using reflection-based constructor invocation.
    /// Supports both public and non-public constructors.
    /// </summary>
    internal class ConstructingProvider : IConstructingProvider
    {
        /// <summary>
        /// Creates an instance of <paramref name="type"/> by invoking its constructor with the supplied arguments.
        /// </summary>
        /// <param name="type">The type to instantiate.</param>
        /// <param name="args">Constructor arguments, matched by position and type.</param>
        /// <returns>A new instance of <paramref name="type"/>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the type cannot be instantiated with the given parameters — for example,
        /// if no matching constructor exists, the type is an interface or abstract, or a constructor argument is invalid.
        /// </exception>
        public object ResolveWithArguments(Type type, params object?[] args)
        {
            if (type.IsInterface || type.IsAbstract)
                throw new InvalidOperationException($"Cannot construct types that are interfaces or abstract {type.Name}.");

            try
            {
                return Activator.CreateInstance(
                    type: type,
                    bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                    binder: null,
                    args: args,
                    culture: null)!;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create {type.Name} with given parameters.", ex);
            }
        }
    }
}
