using System.Reflection;
using FixtureBuilder.AutoConstructingProviders;
using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.ParameterProviders
{
    internal interface IParameterProvider
    {
        object? ResolveParameterValue(ParameterInfo paramInfo, IFixtureContext context, AutoResolveContext autoResolveContext);
    }
}
