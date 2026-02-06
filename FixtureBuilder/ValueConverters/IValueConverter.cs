namespace FixtureBuilder.ValueConverters
{
    internal interface IValueConverter
    {
        object? Convert(Type target, object value);
    }
}
