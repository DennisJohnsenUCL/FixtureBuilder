namespace FixtureBuilder.Tests.Configuration.ValueConverters.Converters.ImplicitConverterTests
{
    internal class ImplicitToIntClass
    {
        public int Number;

        public static implicit operator int(ImplicitToIntClass source) => source.Number;
    }
}
