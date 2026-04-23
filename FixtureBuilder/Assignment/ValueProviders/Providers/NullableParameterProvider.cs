using System.Reflection;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Assignment.ValueProviders.Providers
{
    /// <summary>
    /// Resolves parameter values by returning <c>null</c> when the request source
    /// is a <see cref="ParameterInfo"/> annotated as nullable
    /// and <see cref="FixtureOptions.PreferNullParameterValues"/> is enabled.
    /// </summary>
    internal class NullableParameterProvider : IValueProvider
    {
        public object? ResolveValue(FixtureRequest request, IFixtureContext context)
        {
            if (request.RequestSource is ParameterInfo paramInfo
                && context.OptionsFor(request.RootType).PreferNullParameterValues
                && new NullabilityInfoContext().Create(paramInfo).ReadState == NullabilityState.Nullable)
            {
                return null;
            }

            return new NoResult();
        }
    }
}
