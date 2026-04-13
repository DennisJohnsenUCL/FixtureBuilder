using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Assignment.ValueProviders.Providers
{
    /// <summary>
    /// Provides fixture instances for <see cref="DateTime"/> types.
    /// </summary>
    /// <remarks>
    /// Returns <see cref="DateTime.UtcNow"/> for requests matching <see cref="DateTime"/>;
    /// returns <see langword="null"/> for all other types.
    /// </remarks>
    internal class DateTimeProvider : IValueProvider
    {
        public object? ResolveValue(FixtureRequest request, IFixtureContext context)
        {
            if (request.Type == typeof(DateTime))
            {
                return DateTime.UtcNow;
            }

            return new NoResult();
        }
    }
}
