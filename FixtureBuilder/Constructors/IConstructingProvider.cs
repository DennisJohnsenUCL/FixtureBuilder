namespace FixtureBuilder.Constructors
{
    internal interface IConstructingProvider
    {
        object Resolve(FixtureRequest request, params object?[] args);
    }
}
