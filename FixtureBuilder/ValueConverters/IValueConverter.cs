namespace FixtureBuilder.ValueConverters
{
    public interface IValueConverter
    {
        object? Convert(Type target, object value);
    }
}
