using System.Reflection;

namespace FixtureBuilder.Core
{
    /// <summary>
    /// Represents a request for a value for a constructor parameter, property, or field, containing metadata about the creation context.
    /// </summary>
    public sealed class FixtureRequest
    {
        /// <summary>
        /// Gets or sets the type of the object requested to be created.
        /// </summary>
        public Type Type { get; [Obsolete("Requests should not be modified; this setter will be removed in 4.0.0.")] set; }

        /// <summary>
        /// Gets the declared type of the parameter, property or field in <see cref="RequestSource"/>, with by-ref removed
        /// but before nullable unwrapping and linking.
        /// </summary>
        public Type OriginalType { get; }

        /// <summary>
        /// Gets the source object that initiated the request, which is a <see cref="System.Reflection.PropertyInfo"/>, <see cref="System.Reflection.FieldInfo"/>, or <see cref="System.Reflection.ParameterInfo"/>.
        /// </summary>
        public object RequestSource { get; }

        /// <summary>
        /// Gets the root type of the object graph currently being created.
        /// </summary>
        public Type RootType { get; }

        /// <summary>
        /// Gets the name of the property, field, or parameter in <see cref="RequestSource"/>. This is <see langword="null"/>
        /// for parameters without a name, such as those of multi-dimensional array constructors.
        /// </summary>
        public string? Name { get; }

        internal FixtureRequest(Type type, object requestSource, Type rootType)
        {
            ArgumentNullException.ThrowIfNull(type);
            ArgumentNullException.ThrowIfNull(requestSource);
            ArgumentNullException.ThrowIfNull(rootType);
#pragma warning disable CS0618 // Type or member is obsolete
            Type = type;
#pragma warning restore CS0618 // Type or member is obsolete
            RequestSource = requestSource;
            RootType = rootType;

            (var declared, Name) = requestSource switch
            {
                ParameterInfo pa => (pa.ParameterType, pa.Name),
                PropertyInfo pr => (pr.PropertyType, pr.Name),
                FieldInfo fi => (fi.FieldType, fi.Name),
                _ => throw new ArgumentException($"Invalid RequestSource in FixtureRequest: {requestSource.GetType().Name}", nameof(requestSource))
            };

            OriginalType = declared.IsByRef ? declared.GetElementType()! : declared;
        }
    }
}
