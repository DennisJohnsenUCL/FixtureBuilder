namespace FixtureBuilder.Assignment.ValueProviders
{
    /// <summary>
    /// Creates completed <see cref="Task{TResult}"/> instances for result types that are only known at runtime.
    /// </summary>
    internal static class CompletedTaskFactory
    {
        /// <summary>
        /// Creates a completed <see cref="Task{TResult}"/> holding the default value of the specified result type,
        /// via <see cref="Task.FromResult{TResult}(TResult)"/>.
        /// </summary>
        /// <param name="genericParam">The <c>TResult</c> of the task to create.</param>
        /// <returns>A completed <see cref="Task{TResult}"/> of <paramref name="genericParam"/>.</returns>
        public static object Create(Type genericParam)
        {
            return typeof(Task)
                .GetMethod("FromResult")!
                .MakeGenericMethod(genericParam)
                .Invoke(null, [null])!;
        }
    }
}
