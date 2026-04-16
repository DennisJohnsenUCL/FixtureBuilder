namespace FixtureBuilder.Core
{
    public class FixtureRequest
    {
        public Type Type { get; set; }
        public string? Name { get; }
        public object? RequestSource { get; }

        internal FixtureRequest(Type type)
        {
            ArgumentNullException.ThrowIfNull(type);
            Type = type;
        }

        internal FixtureRequest(Type type, object requestSource, string? name = null)
        {
            ArgumentNullException.ThrowIfNull(type);
            ArgumentNullException.ThrowIfNull(requestSource);
            Type = type;
            RequestSource = requestSource;
            Name = name;
        }
    }
}
