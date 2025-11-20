using FixtureBuilder.Helpers;
using System.Reflection;

namespace FixtureBuilder.Extensions
{
    internal static class TypeExtensions
    {
        public static IEnumerable<DataMemberInfo> GetDataMembers(this Type type, BindingFlags bindingAttr)
        {
            return type.GetProperties(bindingAttr).Select(x => new DataMemberInfo(x))
                .Concat(type.GetFields(bindingAttr).Select(x => new DataMemberInfo(x)));
        }
    }
}
