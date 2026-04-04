namespace FixtureBuilder.ConstructingProviders
{
    internal interface IConstructingProvider
    {
        object ResolveWithArguments(FixtureRequest request, params object?[] args);
    }
}
