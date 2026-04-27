namespace FixtureBuilder.FixtureFactories
{
    public interface ICustomTypeLink
    {
        /// <summary>
        /// Attempts to resolve a concrete type for the given target type.
        /// </summary>
        /// <remarks>
        /// Invoked during the value resolution pipeline to substitute interfaces and abstract types with concrete implementations.
        /// Return <c>null</c> if this link does not handle the given type.
        /// </remarks>
        Type? Link(Type target);
    }
}
