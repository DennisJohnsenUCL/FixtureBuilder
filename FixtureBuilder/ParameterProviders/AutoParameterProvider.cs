using System.Reflection;
using FixtureBuilder.FixtureContexts;

namespace FixtureBuilder.ParameterProviders
{
    /// <summary>
    /// Provides automatic resolution of parameter values using a multi-step fallback strategy.
    /// Implements <see cref="IParameterProvider"/> to resolve constructor or method parameters
    /// within a fixture-building context.
    /// </summary>
    /// <remarks>
    /// The resolution strategy follows this order of precedence:
    /// <list type="number">
    ///   <item><description>If the parameter has a default value, that value is returned.</description></item>
    ///   <item><description>If the parameter is nullable, <see langword="null"/> is returned.</description></item>
    ///   <item><description>The parameter type is resolved through the context's type-linking mechanism,
    ///   and a <see cref="FixtureRequest"/> is created and resolved via <see cref="IFixtureContext.ResolveValue"/>.</description></item>
    ///   <item><description>If no value is resolved, the context's <see cref="IFixtureContext.AutoResolve"/>
    ///   method is used as a final fallback.</description></item>
    /// </list>
    /// </remarks>
    internal class AutoParameterProvider : IParameterProvider
    {
        public object? ResolveParameterValue(ParameterInfo paramInfo, IFixtureContext context, RecursiveResolveContext recursiveResolveContext)
        {
            if (paramInfo.HasDefaultValue && context.Options.PreferDefaultParameterValues)
                return paramInfo.DefaultValue;

            if (new NullabilityInfoContext().Create(paramInfo).ReadState == NullabilityState.Nullable
                && context.Options.PreferNullParameterValues)
                return null;

            var paramType = context.Link(paramInfo.ParameterType) ?? paramInfo.ParameterType;

            var paramRequest = new FixtureRequest(paramType, paramInfo, paramInfo.Name);

            object? result;
            result = context.ResolveValue(paramRequest, context);
            if (result is not NoResult) return result;

            return context.AutoResolve(paramRequest, context, recursiveResolveContext);
        }
    }
}
