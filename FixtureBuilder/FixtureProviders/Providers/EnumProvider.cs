using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.FixtureProviders.Providers
{
    /// <summary>
    /// Provides fixture instances for enumeration types.
    /// </summary>
    /// <remarks>
    /// Returns the first defined value of the requested enum type.
    /// Returns <see langword="null"/> for all non-enum types.
    /// </remarks>
    internal class EnumProvider : IFixtureProvider
    {
        public object? Resolve(FixtureRequest request, IFixtureContext context)
        {
            if (request.Type.IsEnum)
            {
                return Enum.GetValues(request.Type).GetValue(0);
            }

            return null;
        }
    }
}
