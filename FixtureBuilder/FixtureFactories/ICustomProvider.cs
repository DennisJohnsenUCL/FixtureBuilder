#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace FixtureBuilder.Core
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public interface ICustomProvider
    {
        object? ResolveValue(FixtureRequest request);
    }
}
