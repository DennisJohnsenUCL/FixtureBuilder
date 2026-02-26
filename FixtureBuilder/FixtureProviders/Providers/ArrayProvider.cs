using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.FixtureProviders.Providers
{
    internal class ArrayProvider : IFixtureProvider
    {
        public object? Resolve(FixtureRequest request, IFixtureContext context)
        {
            if (request.Type.IsArray)
            {
                var elementType = request.Type.GetElementType()!;
                var instance = Array.CreateInstance(elementType, 10);
                return instance;
            }

            return null;
        }
    }
}
