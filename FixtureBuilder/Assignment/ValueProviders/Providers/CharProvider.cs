using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Assignment.ValueProviders.Providers
{
    /// <summary>
    /// Provides fixture instances for <see cref="char"/> types.
    /// </summary>
    /// <remarks>
    /// Returns a random ASCII letter (A-Z or a-z) for each request.
    /// Returns <see cref="NoResult"/> for all other types.
    /// </remarks>
    internal class CharProvider : IValueProvider
    {
        private const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public object? ResolveValue(FixtureRequest request, IFixtureContext context)
        {
            if (request.Type == typeof(char))
            {
                return letters[Random.Shared.Next(letters.Length)];
            }

            return NoResult.Instance;
        }
    }
}
