namespace FixtureBuilder.Core.FixtureContexts
{
    internal interface IOptionsContext
    {
        FixtureOptions Options { set; }
        FixtureOptions GetBaseOptions();
        void SetOptions(Action<FixtureOptions> action);
        void AddRootOptions(Type type, FixtureOptions options);
        FixtureOptions OptionsFor(Type rootType);
    }
}
