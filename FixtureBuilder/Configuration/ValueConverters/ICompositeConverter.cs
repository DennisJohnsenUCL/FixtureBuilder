namespace FixtureBuilder.Configuration.ValueConverters
{
    internal interface ICompositeConverter : IValueConverter
    {
        void AddConverter(IValueConverter converter);
    }
}
