using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Assignment.ValueProviders.Providers
{
    /// <summary>
    /// Provides fixture instances for <see cref="Task"/> types.
    /// </summary>
    /// <remarks>
    /// Returns <see cref="Task.CompletedTask"/> for each request, since an auto-constructed task is never started and awaiting it would hang.
    /// Returns <see cref="NoResult"/> for all other types, including <see cref="Task{TResult}"/>.
    /// </remarks>
    internal class TaskProvider : IValueProvider
    {
        public object? ResolveValue(FixtureRequest request, IFixtureContext context)
        {
            if (request.Type == (typeof(Task)))
            {
                return Task.CompletedTask;
            }

            return NoResult.Instance;
        }
    }
}
