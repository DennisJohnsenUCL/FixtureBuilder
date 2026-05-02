namespace FixtureBuilder.Core
{
    /// <summary>
    /// Represents a request to create an object of a specific type, containing metadata about the creation context.
    /// </summary>
    public sealed class FixtureRequest
    {
        /// <summary>
        /// Gets or sets the type of the object requested to be created.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Gets the source object that initiated the request, such as the <see cref="System.Reflection.PropertyInfo"/>, <see cref="System.Reflection.FieldInfo"/>, or <see cref="System.Reflection.ParameterInfo"/>.
        /// </summary>
        public object? RequestSource { get; }

        /// <summary>
        /// Gets the root type of the object graph currently being created.
        /// </summary>
        public Type RootType { get; }

        /// <summary>
        /// Gets the name associated with the request, such as a property, field, or parameter name.
        /// </summary>
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
