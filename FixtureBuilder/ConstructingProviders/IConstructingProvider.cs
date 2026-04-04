namespace FixtureBuilder.ConstructingProviders
{
    internal interface IConstructingProvider
    {
        object Resolve(FixtureRequest request, params object?[] args);
    }
}
