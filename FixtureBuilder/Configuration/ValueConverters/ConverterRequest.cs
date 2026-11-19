namespace FixtureBuilder.Configuration.ValueConverters
{
    internal readonly struct ConverterRequest
    {
        public Type Type { get; }
        public Type RootType { get; }
        public Type OriginalType { get; }

        public ConverterRequest(Type type, Type rootType, Type originalType)
        {
            Type = type;
            RootType = rootType;
            OriginalType = originalType;
        }
    }
}
