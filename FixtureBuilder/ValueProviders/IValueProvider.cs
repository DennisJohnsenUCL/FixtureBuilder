using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.ValueProviders
{
    internal interface IValueProvider
    {
        object? ResolveValue(FixtureRequest request, IFixtureContext context);
    }
}
