using Bogus;
using FixtureBuilder.Core;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace FixtureBuilder.Bogus
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public interface IBogusCustomProvider
    {
        object? ResolveValue(FixtureRequest request, Faker faker);
    }
}
