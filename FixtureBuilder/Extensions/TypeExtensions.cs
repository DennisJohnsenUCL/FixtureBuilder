using System.Collections;
using System.Collections.Immutable;
using System.Reflection;

namespace FixtureBuilder.Extensions
{
    internal static class TypeExtensions
    {
        extension(Type type)
        {
            /// <summary>
            /// Returns a sequence of <see cref="DataMemberInfo"/> instances representing all properties and fields
            /// of the specified type that match the given binding constraints.
            /// </summary>
            /// <param name="bindingAttr">
            /// A bitwise combination of <see cref="BindingFlags"/> that specifies how the search for members is conducted.
            /// </param>
            /// <returns>
            /// An <see cref="IEnumerable{DataMemberInfo}"/> containing the type's matching properties and
            /// its matching fields, each wrapped in a <see cref="DataMemberInfo"/>.
            /// </returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is <see langword="null"/>.</exception>
            public IEnumerable<DataMemberInfo> GetDataMembers(BindingFlags bindingAttr)
            {
                ArgumentNullException.ThrowIfNull(type);

                return type.GetProperties(bindingAttr).Select(p => new DataMemberInfo(p))
                    .Concat(type.GetFields(bindingAttr).Select(f => new DataMemberInfo(f)));
            }

            /// <summary>
            /// Returns the generic type definition of the specified type, or <see langword="null"/> if the type is not generic.
            /// </summary>
            /// <returns>
            /// The generic type definition if <paramref name="type"/> is a generic type; otherwise, <see langword="null"/>.
            /// </returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is <see langword="null"/>.</exception>
            public Type? GetGenericTypeDefinitionOrDefault()
            {
                ArgumentNullException.ThrowIfNull(type);

                return type.IsGenericType ? type.GetGenericTypeDefinition() : null;
            }

            /// <summary>
            /// Removes any by-ref and <see cref="Nullable{T}"/> wrapping from the specified type.
            /// </summary>
            /// <returns>
            /// The type wrapped by <paramref name="type"/> if it is a by-ref type, a <see cref="Nullable{T}"/>,
            /// or a by-ref <see cref="Nullable{T}"/>; otherwise, <paramref name="type"/> itself.
            /// </returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is <see langword="null"/>.</exception>
            public Type Unwrap()
            {
                ArgumentNullException.ThrowIfNull(type);

                var unwrappedByRef = type.IsByRef ? type.GetElementType()! : type;
                var unwrappedNullable = Nullable.GetUnderlyingType(unwrappedByRef) ?? unwrappedByRef;
                return unwrappedNullable;
            }

            /// <summary>
            /// Determines whether the specified type implements a given interface, supporting both
            /// closed and open generic interface definitions.
            /// </summary>
            /// <param name="target">
            /// The interface type to check for. This can be a concrete interface (e.g., <see cref="IDisposable"/>),
            /// a closed generic interface (e.g., <c>IEnumerable&lt;string&gt;</c>), or an open generic type
            /// definition (e.g., <c>IEnumerable&lt;&gt;</c>).
            /// </param>
            /// <returns>
            /// <see langword="true"/> if <paramref name="type"/> implements <paramref name="target"/>;
            /// otherwise, <see langword="false"/>.
            /// </returns>
            /// <exception cref="ArgumentNullException">
            /// Thrown when <paramref name="type"/> or <paramref name="target"/> is <see langword="null"/>.
            /// </exception>
            /// <exception cref="ArgumentException">
            /// Thrown when <paramref name="target"/> is not an interface type.
            /// </exception>
            public bool Implements(Type target)
            {
                ArgumentNullException.ThrowIfNull(type);
                ArgumentNullException.ThrowIfNull(target);

                if (!target.IsInterface) throw new ArgumentException("Target type must be an interface", nameof(target));

                return type.GetInterfaces().Any(i =>
                i == target
                || (target.IsGenericTypeDefinition
                && i.GetGenericTypeDefinitionOrDefault() == target));
            }

            /// <summary>
            /// Retrieves the element type of a type that implements <see cref="IEnumerable{T}"/>.
            /// </summary>
            /// <returns>
            /// The element type <c>T</c> if <paramref name="type"/> implements <see cref="IEnumerable{T}"/>;
            /// otherwise, <see langword="null"/>.
            /// </returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is <see langword="null"/>.</exception>
            public Type? GetEnumerableElementType()
            {
                ArgumentNullException.ThrowIfNull(type);

                return type.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    ?.GenericTypeArguments[0];
            }

            /// <summary>
            /// Determines whether the specified type is a dictionary type. This includes types that implement
            /// <see cref="IDictionary"/>, as well as generic interface types matching
            /// <see cref="IDictionary{TKey, TValue}"/>, <see cref="IReadOnlyDictionary{TKey, TValue}"/>,
            /// or <see cref="IImmutableDictionary{TKey, TValue}"/>.
            /// </summary>
            /// <returns>
            /// <see langword="true"/> if <paramref name="type"/> is a dictionary type; otherwise, <see langword="false"/>.
            /// </returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is <see langword="null"/>.</exception>
            public bool IsDictionary()
            {
                ArgumentNullException.ThrowIfNull(type);

                if (typeof(IDictionary).IsAssignableFrom(type)) return true;

                else if (type.IsInterface && type.GetGenericTypeDefinitionOrDefault() is { } genericTypeDef)
                {
                    if (genericTypeDef == typeof(IDictionary<,>) ||
                        genericTypeDef == typeof(IReadOnlyDictionary<,>) ||
                        genericTypeDef == typeof(IImmutableDictionary<,>))
                        return true;
                }

                return false;
            }

            /// <summary>
            /// Extracts the key and value types from a dictionary type by inspecting its
            /// <see cref="IEnumerable{T}"/> implementation for a <see cref="KeyValuePair{TKey, TValue}"/>
            /// element type.
            /// </summary>
            /// <returns>
            /// A tuple containing the key and value types of the dictionary. Returns
            /// <c>(null, null)</c> if <paramref name="type"/> is not a dictionary type.
            /// Returns <c>(typeof(object), typeof(object))</c> if the type is a dictionary but does not
            /// implement <see cref="IEnumerable{T}"/> (e.g., non-generic dictionaries such as <see cref="Hashtable"/>).
            /// </returns>
            /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is <see langword="null"/>.</exception>
            public (Type? KeyType, Type? ValueType) GetDictionaryEnumerableTypes()
            {
                ArgumentNullException.ThrowIfNull(type);

                if (!type.IsDictionary()) return (null, null);

                if (!type.Implements(typeof(IEnumerable<>))) return (typeof(object), typeof(object));

                foreach (var i in type.GetInterfaces())
                {
                    if (i.GetGenericTypeDefinitionOrDefault() != typeof(IEnumerable<>))
                        continue;

                    var arg = i.GetGenericArguments()[0];
                    if (arg.GetGenericTypeDefinitionOrDefault() != typeof(KeyValuePair<,>))
                        continue;

                    var kvpArgs = arg.GetGenericArguments();
                    return (kvpArgs[0], kvpArgs[1]);
                }
                return (null, null);
            }
        }
    }
}
