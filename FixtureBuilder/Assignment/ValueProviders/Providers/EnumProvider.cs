using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Assignment.ValueProviders.Providers
{
    /// <summary>
    /// Provides fixture instances for enumeration types.
    /// </summary>
    /// <remarks>
    /// Returns the first defined value of the requested enum type.
    /// Returns <see langword="null"/> for all non-enum types.
    /// </remarks>
    internal class EnumProvider : IValueProvider
    {
        public object? ResolveValue(FixtureRequest request, IFixtureContext context)
        {
            if (request.Type.IsEnum)
            {
                return Enum.GetValues(request.Type).GetValue(0);
            }

            return new NoResult();
        }
    }
}
