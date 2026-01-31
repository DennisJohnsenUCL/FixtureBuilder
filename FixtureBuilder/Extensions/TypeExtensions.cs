using FixtureBuilder.Helpers;
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

        public static Type? GetGenericTypeDefinitionOrDefault(this Type type)
        {
            return type.IsGenericType ? type.GetGenericTypeDefinition() : null;
        }
    }
}
