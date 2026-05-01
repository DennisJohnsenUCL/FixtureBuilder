namespace FixtureBuilder.FixtureFactories
{
    /// <summary>
    /// Defines a custom type substitution rule for the value resolution pipeline.
    /// </summary>
    public interface ICustomTypeLink
    {
        /// <summary>
        /// Attempts to resolve a concrete type for the given target type.
        /// </summary>
        /// <remarks>
        /// Invoked during the value resolution pipeline to substitute interfaces and abstract types with concrete implementations.
        /// Return <see langword="null"/>null if this link does not handle the given type.
        /// </remarks>
        Type? Link(Type target);
    }
}
