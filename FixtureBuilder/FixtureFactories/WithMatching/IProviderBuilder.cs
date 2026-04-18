namespace FixtureBuilder.FixtureFactories.WithMatching
{
    internal interface IProviderBuilder<TReturn>
    {
        TReturn With<T>(T value);
        TReturn With<T>(Func<T> func);
        TReturn With<T>(T value, string name);
        TReturn With<T>(Func<T> func, string name);
        TReturn WithParameter<T>(T value);
        TReturn WithParameter<T>(Func<T> func);
        TReturn WithParameter<T>(T value, string name);
        TReturn WithParameter<T>(Func<T> func, string name);
        TReturn WithPropertyOrField<T>(T value);
        TReturn WithPropertyOrField<T>(Func<T> func);
        TReturn WithPropertyOrField<T>(T value, string name);
        TReturn WithPropertyOrField<T>(Func<T> func, string name);
    }
}
