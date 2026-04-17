namespace FixtureBuilder.Configuration.ValueConverters
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
