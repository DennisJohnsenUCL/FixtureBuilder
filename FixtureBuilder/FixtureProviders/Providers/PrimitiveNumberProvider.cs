using System.Collections.Immutable;
using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.FixtureProviders.Providers
{
    /// <summary>
    /// Provides fixture instances for primitive numeric types.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Returns a random value between 1 and 10 (inclusive), converted to the requested numeric type.
    /// Supported types include:
    /// </para>
    /// <list type="bullet">
    ///   <item><description><see cref="byte"/>, <see cref="sbyte"/></description></item>
    ///   <item><description><see cref="short"/>, <see cref="ushort"/></description></item>
    ///   <item><description><see cref="int"/>, <see cref="uint"/></description></item>
    ///   <item><description><see cref="long"/>, <see cref="ulong"/></description></item>
    ///   <item><description><see cref="float"/>, <see cref="double"/>, <see cref="decimal"/></description></item>
    /// </list>
    /// <para>
    /// Returns <see langword="null"/> for any type not in the supported set.
    /// </para>
    /// </remarks>
    internal class PrimitiveNumberProvider : IFixtureProvider
    {
        private readonly IEnumerable<Type> _types = [
            typeof(byte), typeof(sbyte),
            typeof(short), typeof(ushort),
            typeof(int), typeof(uint),
            typeof(long), typeof(ulong),
            typeof(float), typeof(double), typeof(decimal)
            ];

        public object? Resolve(FixtureRequest request, IFixtureContext context)
        {
            if (_types.Contains(request.Type))
            {
                return Convert.ChangeType(Random.Shared.Next(1, 11), request.Type);
            }

            return null;
        }
    }
}
