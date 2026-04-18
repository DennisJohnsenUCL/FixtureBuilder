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
            RootType = Type;
        }

        internal FixtureRequest(Type type, object requestSource, Type rootType, string? name) : this(type)
        {
            ArgumentNullException.ThrowIfNull(requestSource);
            ArgumentNullException.ThrowIfNull(rootType);
            RequestSource = requestSource;
            RootType = rootType;
            Name = name;
        }
    }
}
