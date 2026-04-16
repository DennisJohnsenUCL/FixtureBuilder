namespace FixtureBuilder.Configuration.ValueConverters.ConverterBuilders
{
    internal class ConverterGraph
    {
        public IValueConverter Root { get; }
        public ICompositeConverter Composite { get; }

        public ConverterGraph(IValueConverter root, ICompositeConverter composite)
        {
            ArgumentNullException.ThrowIfNull(root);
            ArgumentNullException.ThrowIfNull(composite);
            Root = root;
            Composite = composite;
        }
    }
}
