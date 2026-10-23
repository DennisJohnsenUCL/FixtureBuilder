namespace FixtureBuilder.Assignment.ValueProviders
{
    internal interface ICompositeValueProvider : IValueProvider
    {
        void AddProvider(IValueProvider provider);
        void AddRootProvider(IValueProvider provider);
    }
}
