using Bogus;
using FixtureBuilder.Core;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace FixtureBuilder.Bogus
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Defines a Bogus-integrated custom value provider for the value resolution pipeline.
    /// </summary>
    public interface IBogusCustomProvider
    {
        /// <summary>
        /// Attempts to resolve a value for the given request using a <see cref="Faker"/> instance for data generation.
        /// </summary>
        /// <remarks>
        /// Return <see cref="NoResult"/> if the provider does not handle the given request. A <see langword="null"/> return is treated as an explicit null assignment.
        /// </remarks>
        object? ResolveValue(FixtureRequest request, Faker faker);
    }
}
