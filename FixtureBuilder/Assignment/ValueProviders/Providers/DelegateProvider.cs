using System.Linq.Expressions;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Assignment.ValueProviders.Providers
{
    /// <summary>
    /// Provides fixture instances for delegate types.
    /// </summary>
    /// <remarks>
    /// Returns a compiled delegate that ignores its arguments and returns the default value of its return type.
    /// Throws an <see cref="InvalidOperationException"/> for delegates that return by reference.
    /// Returns <see cref="NoResult"/> for all other types.
    /// </remarks>
    internal class DelegateProvider : IValueProvider
    {
        public object? ResolveValue(FixtureRequest request, IFixtureContext context)
        {
            if (request.Type.IsSubclassOf(typeof(MulticastDelegate)))
            {
                var invoke = request.Type.GetMethod("Invoke")!;

                if (invoke.ReturnType.IsByRef)
                    throw new InvalidOperationException($"Cannot create {request.Type.Name}, delegates that return by reference are not supported.");

                var parameters = invoke.GetParameters().Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToArray();
                return Expression.Lambda(request.Type, Expression.Default(invoke.ReturnType), parameters).Compile();
            }

            return new NoResult();
        }
    }
}
