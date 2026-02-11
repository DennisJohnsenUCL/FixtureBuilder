using System.Collections;

namespace FixtureBuilder.Helpers
{
    internal static class CollectionHelpers
    {
        private static bool ElementTypeIsAssignable(Type fieldType, Type sourceElementType)
        {
            //TODO: Save to snippet somewhere, or use, but update to GetEnumerableElementType
            if (!typeof(IEnumerable).IsAssignableFrom(fieldType)) return false;

            Type? fieldElementType;
            if (fieldType.IsGenericType) fieldElementType = fieldType.GetGenericArguments()[0];
            else if (fieldType.IsArray) fieldElementType = fieldType.GetElementType();
            else return true;

            if (fieldElementType != null && (fieldElementType == sourceElementType || fieldElementType.IsAssignableFrom(sourceElementType))) return true;
            else return false;
        }
    }
}
