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

        public FixtureRequest(Type type, object requestSource, string? name = null)
        {
            ArgumentNullException.ThrowIfNull(type);
            ArgumentNullException.ThrowIfNull(requestSource);
            Type = type;
            RequestSource = requestSource;
            Name = name;
        }
    }
}
