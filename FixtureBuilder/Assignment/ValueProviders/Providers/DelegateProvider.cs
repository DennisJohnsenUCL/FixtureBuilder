using System.Linq.Expressions;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Assignment.ValueProviders.Providers
{
    /// <summary>
    /// Provides fixture instances for delegate types.
    /// </summary>
    /// <remarks>
    /// Returns a compiled delegate that ignores its arguments and returns the default value of its return type,
    /// except for <see cref="Task"/> and <see cref="Task{TResult}"/>, where it returns a completed task, holding the default value of <c>TResult</c> for the latter.
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

                Expression? body = null;
                if (invoke.ReturnType == typeof(Task)) body = Expression.Constant(Task.CompletedTask);
                else if (invoke.ReturnType.GetGenericTypeDefinitionOrDefault() == typeof(Task<>))
                {
                    body = Expression.Constant(CompletedTaskFactory.Create(invoke.ReturnType.GenericTypeArguments[0]));
                }
                else body = Expression.Default(invoke.ReturnType);

                return Expression.Lambda(request.Type, body, parameters).Compile();
            }

            return NoResult.Instance;
        }
    }
}
