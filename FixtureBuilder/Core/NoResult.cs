namespace FixtureBuilder.Core
{
    /// <summary>
    /// Represents the absence of a result. This is used by providers and converters
    /// to indicate that they cannot process the current request or provide a valid value.
    /// Evaluating pipeline components check for this object to delegate responsibility to the next capable component.
    /// An <see cref="ICustomProvider"/> or <see cref="ICustomConverter"/> must return <see cref="Instance"/> rather than <see langword="null"/> when it does not handle the incoming request.
    /// </summary>
    public sealed class NoResult
    {
        /// <summary>
        /// Gets the shared <see cref="NoResult"/> instance.
        /// </summary>
#pragma warning disable CS0618 // Type or member is obsolete
        public static NoResult Instance { get; } = new();
#pragma warning restore CS0618 // Type or member is obsolete

        /// <summary>
        /// Initializes a new <see cref="NoResult"/>. Use <see cref="Instance"/> instead.
        /// </summary>
        [Obsolete("Use NoResult.Instance instead; the public constructor will be removed in 4.0.0.")]
        public NoResult() { }
    }
}
