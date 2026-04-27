#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace FixtureBuilder.Core
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public interface ICustomProvider
    {
        /// <summary>
        /// Attempts to resolve a value for the given request.
        /// </summary>
        /// <remarks>
        /// Return <c>new NoResult()</c> if the provider does not handle the given request. A <c>null</c> return is treated as an explicit null assignment.
        /// </remarks>
        object? ResolveValue(FixtureRequest request);
    }
}
