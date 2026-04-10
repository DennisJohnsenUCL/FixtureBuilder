namespace FixtureBuilder.FixtureContexts
{
    internal interface IOptionsContext
    {
        FixtureOptions Options { get; set; }
        void SetOptions(Action<FixtureOptions> action);
    }
}
