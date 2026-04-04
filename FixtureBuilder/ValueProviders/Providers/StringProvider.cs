using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.ValueProviders.Providers
{
    /// <summary>
    /// Provides fixture instances for <see cref="string"/> types.
    /// </summary>
    /// <remarks>
    /// Returns <see cref="FixtureRequest.Name"/> if it is not <see langword="null"/>;
    /// otherwise returns an empty string. Returns <see langword="null"/> for all non-string types.
    /// </remarks>
    internal class StringProvider : IValueProvider
    {
        public object? ResolveValue(FixtureRequest request, IFixtureContext context)
        {
            if (request.Type == typeof(string))
            {
                if (request.Name != null) return request.Name;
                return "";
            }

            return null;
        }
    }
}
