using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Assignment.ValueProviders
{
    internal interface IValueProvider
    {
        object? ResolveValue(FixtureRequest request, IFixtureContext context);
    }
}
