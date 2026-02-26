namespace FixtureBuilder
{
    internal class FixtureRequest
    {
        public Type Type { get; set; }
        public string? Name { get; }
        public object? RequestSource { get; }

        public FixtureRequest(Type type)
        {
            ArgumentNullException.ThrowIfNull(type);
            Type = type;
        }

        public FixtureRequest(Type type, object requestSource)
        {
            ArgumentNullException.ThrowIfNull(type);
            ArgumentNullException.ThrowIfNull(requestSource);
            Type = type;
            RequestSource = requestSource;
        }

        public FixtureRequest(Type type, string name, object requestSource)
        {
            ArgumentNullException.ThrowIfNull(type);
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(requestSource);
            Type = type;
            Name = name;
            RequestSource = requestSource;
        }
    }
}
