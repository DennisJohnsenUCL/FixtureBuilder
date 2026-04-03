using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.ValueProviders
{
    internal interface IValueProvider
    {
        object? Resolve(FixtureRequest request, IFixtureContext context);
    }
}
