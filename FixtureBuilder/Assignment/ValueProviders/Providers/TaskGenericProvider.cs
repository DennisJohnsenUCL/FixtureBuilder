using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Assignment.ValueProviders.Providers
{
    /// <summary>
    /// Provides fixture instances for <see cref="Task{TResult}"/> types.
    /// </summary>
    /// <remarks>
    /// Returns a completed task holding the default value of <c>TResult</c>, created via <see cref="Task.FromResult{TResult}(TResult)"/>.
    /// Returns <see cref="NoResult"/> for all other types, including the non-generic <see cref="Task"/>.
    /// </remarks>
    internal class TaskGenericProvider : IValueProvider
    {
        public object? ResolveValue(FixtureRequest request, IFixtureContext context)
        {
            if (request.Type.GetGenericTypeDefinitionOrDefault() is { } typeDef && typeDef == typeof(Task<>))
            {
                var genericParam = request.Type.GenericTypeArguments[0];
                return typeof(Task)
                    .GetMethod("FromResult")!
                    .MakeGenericMethod(genericParam)
                    .Invoke(null, [null]);
            }

            return NoResult.Instance;
        }
    }
}
