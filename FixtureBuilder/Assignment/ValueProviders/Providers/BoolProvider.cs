using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Assignment.ValueProviders.Providers
{
    /// <summary>
    /// Provides fixture instances for <see cref="bool"/> types.
    /// </summary>
    /// <remarks>
    /// Returns <see langword="true"/> for each request.
    /// Returns <see cref="NoResult"/> for all other types.
    /// </remarks>
    internal class BoolProvider : IValueProvider
    {
        public object? ResolveValue(FixtureRequest request, IFixtureContext context)
        {
            if (request.Type == typeof(bool))
            {
                return true;
            }

            return NoResult.Instance;
        }
    }
}
