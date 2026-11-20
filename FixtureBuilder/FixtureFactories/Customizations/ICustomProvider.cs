#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace FixtureBuilder.Core
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Defines a custom value provider for the value resolution pipeline.
    /// </summary>
    public interface ICustomProvider
    {
        /// <summary>
        /// Attempts to resolve a value for the given request.
        /// </summary>
        /// <remarks>
        /// Return <see cref="NoResult.Instance"/> if the provider does not handle the given request. A <see langword="null"/> return is treated as an explicit null assignment.
        /// If the provider returns a cached instance of a mutable reference type, that instance is shared by every fixture that receives it,
        /// and configuring its members changes it for all of them. Return a new instance per call unless sharing is intended.
        /// </remarks>
        object? ResolveValue(FixtureRequest request);
    }
}
