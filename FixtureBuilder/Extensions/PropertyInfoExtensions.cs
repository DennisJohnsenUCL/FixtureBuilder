using System.Reflection;

namespace FixtureBuilder.Extensions
{
    internal static class PropertyInfoExtensions
    {
        public static bool IsStatic(this PropertyInfo propertyInfo, bool nonPublic = false)
        {
            return propertyInfo.GetAccessors(nonPublic).Any(mi => mi.IsStatic);
        }
    }
}
