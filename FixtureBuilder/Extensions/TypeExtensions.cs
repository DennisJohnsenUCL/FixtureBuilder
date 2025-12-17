using FixtureBuilder.Helpers;
using System.Reflection;

namespace FixtureBuilder.Extensions
{
    internal static class TypeExtensions
    {
        internal static IEnumerable<DataMemberInfo> GetDataMembers(this Type type, BindingFlags bindingAttr)
        {
            return type.GetProperties(bindingAttr).Select(p => new DataMemberInfo(p))
                .Concat(type.GetFields(bindingAttr).Select(f => new DataMemberInfo(f)));
        }
    }
}
