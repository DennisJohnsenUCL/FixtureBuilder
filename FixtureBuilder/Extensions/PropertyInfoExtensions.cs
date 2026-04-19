using System.Reflection;

namespace FixtureBuilder.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="PropertyInfo"/>.
    /// </summary>
    internal static class PropertyInfoExtensions
    {
        extension(PropertyInfo propertyInfo)
        {
            /// <summary>
            /// Determines whether the property is static by checking if any of its accessors are static.
            /// </summary>
            /// <param name="propertyInfo">The <see cref="PropertyInfo"/> instance to check.</param>
            /// <param name="nonPublic">
            /// <see langword="true"/> to include non-public accessors in the check;
            /// <see langword="false"/> to consider only public accessors. The default is <see langword="false"/>.
            /// </param>
            /// <returns>
            /// <see langword="true"/> if any of the property's accessors are static; otherwise, <see langword="false"/>.
            /// </returns>
            public bool IsStatic(bool nonPublic = false)
            {
                return propertyInfo.GetAccessors(nonPublic).Any(mi => mi.IsStatic);
            }
        }
    }
}
