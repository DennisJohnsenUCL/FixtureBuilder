using System.Reflection;
using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.ParameterProviders
{
    internal interface IParameterProvider
    {
        object? ResolveParameterValue(ParameterInfo paramInfo, IFixtureContext context);
    }
}
