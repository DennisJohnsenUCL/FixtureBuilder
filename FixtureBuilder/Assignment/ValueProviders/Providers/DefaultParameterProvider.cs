using System.Reflection;
using FixtureBuilder.Assignment.ValueProviders;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Assignment.ValueProviders.Providers
{
    /// <summary>
    /// Resolves parameter values by returning the parameter's declared default value
    /// when the request source is a <see cref="ParameterInfo"/> with a default value
    /// and <see cref="FixtureOptions.PreferDefaultParameterValues"/> is enabled.
    /// </summary>
    internal class DefaultParameterProvider : IValueProvider
    {
        public object? ResolveValue(FixtureRequest request, IFixtureContext context)
        {
            if (request.RequestSource is ParameterInfo paramInfo
                && paramInfo.HasDefaultValue
                && context.Options.PreferDefaultParameterValues)
            {
                return paramInfo.DefaultValue;
            }

            return new NoResult();
        }
    }
}
