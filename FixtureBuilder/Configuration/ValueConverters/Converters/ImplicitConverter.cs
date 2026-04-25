using System.Reflection;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Configuration.ValueConverters.Converters
{
    /// <summary>
    /// Converts a value to a target type using implicit conversion operators (op_Implicit).
    /// Searches for implicit operators on the source type first, then the target type.
    /// Requires <see cref="FixtureOptions.AllowImplicitConversion"/> to be enabled.
    /// </summary>
    internal class ImplicitConverter : IValueConverter
    {
        public object? Convert(FixtureRequest request, object value, IFixtureContext context)
        {
            if (context.OptionsFor(request.RootType).AllowImplicitConversion)
            {
                var sourceType = value.GetType();
                var target = request.Type;

                var implConversionMethod = FindImplicit(sourceType, sourceType, target)
                    ?? FindImplicit(target, sourceType, target);

                if (implConversionMethod == null) return new NoResult();

                return implConversionMethod.Invoke(null, [value]);
            }

            return new NoResult();
        }

        private static MethodInfo? FindImplicit(Type searchType, Type parameterType, Type returnType)
        {
            return searchType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m =>
                    m.Name == "op_Implicit" &&
                    m.ReturnType == returnType &&
                    m.GetParameters() is { Length: 1 } p &&
                    p[0].ParameterType.IsAssignableFrom(parameterType));
        }
    }
}
