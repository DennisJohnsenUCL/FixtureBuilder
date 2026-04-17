namespace FixtureBuilder.FixtureFactories.WithMatching
{
    internal interface IProviderBuilder<TReturn>
    {
        TReturn With<T>(T value);
        TReturn With<T>(T value, string name);
        TReturn WithParameter<T>(T value);
        TReturn WithParameter<T>(T value, string name);
        TReturn WithPropertyOrField<T>(T value);
        TReturn WithPropertyOrField<T>(T value, string name);
        TReturn WithNamed(string name, object? value);
    }
}
