namespace FixtureBuilder.Core
{
    internal readonly struct TypeRequest
    {
        public Type Type { get; }
        public Type RootType { get; }

        public TypeRequest(Type type, Type rootType)
        {
            ArgumentNullException.ThrowIfNull(type);
            ArgumentNullException.ThrowIfNull(rootType);

            Type = type;
            RootType = rootType;
        }
    }
}
