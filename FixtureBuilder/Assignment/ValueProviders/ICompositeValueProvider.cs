namespace FixtureBuilder.Assignment.ValueProviders
{
    internal interface ICompositeValueProvider : IValueProvider
    {
        void AddProvider(IValueProvider provider);
    }
}
