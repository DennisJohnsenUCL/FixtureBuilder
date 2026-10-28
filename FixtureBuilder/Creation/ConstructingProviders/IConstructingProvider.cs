namespace FixtureBuilder.Creation.ConstructingProviders
{
    internal interface IConstructingProvider
    {
        object ResolveWithArguments(Type type, params object?[] args);
    }
}
