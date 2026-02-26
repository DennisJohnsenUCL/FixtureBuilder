using FixtureBuilder.FixtureContexts;
using System.Collections.Immutable;

namespace FixtureBuilder.FixtureProviders.Providers
{
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
