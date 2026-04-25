namespace FixtureBuilder.Core
{
    public class FixtureRequest
    {
        public Type Type { get; set; }
        public object? RequestSource { get; }
        public Type RootType { get; }
        public string? Name { get; }

        internal FixtureRequest(Type type)
        {
            ArgumentNullException.ThrowIfNull(type);
            Type = type;
            RootType = type;
        }

        internal FixtureRequest(Type type, Type rootType) : this(type)
        {
            ArgumentNullException.ThrowIfNull(rootType);
            RootType = rootType;
        }

        internal FixtureRequest(Type type, object requestSource, Type rootType, string? name) : this(type, rootType)
        {
            ArgumentNullException.ThrowIfNull(requestSource);
            RequestSource = requestSource;
            Name = name;
        }
    }
}
