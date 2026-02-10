using FixtureBuilder.Helpers;
using System.Collections;
using System.Collections.Immutable;
using System.Reflection;

namespace FixtureBuilder.Extensions
{
    internal static class TypeExtensions
    {
        public static IEnumerable<DataMemberInfo> GetDataMembers(this Type type, BindingFlags bindingAttr)
        {
            return type.GetProperties(bindingAttr).Select(p => new DataMemberInfo(p))
                .Concat(type.GetFields(bindingAttr).Select(f => new DataMemberInfo(f)));
        }

        public static bool TryGetGenericTypeDefinition(this Type type, out Type genericTypeDefinition)
        {
            if (type.IsGenericType)
            {
                genericTypeDefinition = type.GetGenericTypeDefinition();
                return true;
            }
            genericTypeDefinition = null!;
            return false;
        }

        public static Type? GetGenericTypeDefinitionOrDefault(this Type type)
        {
            return type.IsGenericType ? type.GetGenericTypeDefinition() : null;
        }

        public static bool Implements(this Type type, Type target)
        {
            ArgumentNullException.ThrowIfNull(type);
            ArgumentNullException.ThrowIfNull(target);

            if (!target.IsInterface) throw new ArgumentException("Target type must be an interface", nameof(target));

            return type.GetInterfaces().Any(i =>
            i == target ||
            (target.IsGenericTypeDefinition && i.IsGenericType &&
            i.GetGenericTypeDefinition() == target));
        }

        public static Type? GetEnumerableElementType(this Type type)
        {
            return type.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                ?.GenericTypeArguments[0];
        }

        public static bool IsDictionary(this Type type)
        {
            if (typeof(IDictionary).IsAssignableFrom(type)) return true;

            else if (type.IsInterface && type.TryGetGenericTypeDefinition(out var genericTypeDef))
            {
                if (genericTypeDef == typeof(IDictionary<,>) ||
                    genericTypeDef == typeof(IReadOnlyDictionary<,>) ||
                    genericTypeDef == typeof(IImmutableDictionary<,>))
                    return true;
            }

            return false;
        }
    }
}
