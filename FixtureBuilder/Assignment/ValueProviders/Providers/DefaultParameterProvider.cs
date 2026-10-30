using System.Reflection;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Extensions;

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
                && context.OptionsFor(request.RootType).PreferDefaultParameterValues)
            {
                var type = request.OriginalType;
                var value = paramInfo.DefaultValue;

                if (value is null)
                    return type.GetDefaultValue();

                if (Nullable.GetUnderlyingType(type) is { IsEnum: true } enumType)
                    return Enum.ToObject(enumType, value);

                return value;
            }

            return NoResult.Instance;
        }
    }
}
