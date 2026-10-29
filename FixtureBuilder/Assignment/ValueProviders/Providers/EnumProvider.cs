using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Assignment.ValueProviders.Providers
{
    /// <summary>
    /// Provides fixture instances for enumeration types.
    /// </summary>
    /// <remarks>
    /// Returns the first defined value of the requested enum type, or 0 if the enum has no members.
    /// Returns <see cref="NoResult"/> for all non-enum types.
    /// </remarks>
    internal class EnumProvider : IValueProvider
    {
        public object? ResolveValue(FixtureRequest request, IFixtureContext context)
        {
            if (request.Type.IsEnum)
            {
                var values = Enum.GetValues(request.Type);

                return values.Length > 0
                    ? values.GetValue(0)
                    : Enum.ToObject(request.Type, 0);
            }

            return NoResult.Instance;
        }
    }
}
