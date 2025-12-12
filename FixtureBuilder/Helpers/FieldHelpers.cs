using System.Collections;
using System.Reflection;

namespace FixtureBuilder.Helpers
{
    internal static class FieldHelpers
    {
        public static void SetField(object instance, string fieldName, object value, bool allowNonCollection)
            => SetField(instance, fieldName, [value], allowNonCollection);

        public static void SetField(object instance, string fieldName, IEnumerable<object> values, bool allowNonCollection)
        {
            if (!TryGetField(instance.GetType(), fieldName, out var fieldInfo))
                throw new InvalidOperationException($"Field '{fieldName}' not found on {instance.GetType().Name}.");

            var fieldType = fieldInfo.FieldType;

            if (typeof(IEnumerable).IsAssignableFrom(fieldType) && fieldType != typeof(string))
            {
                CollectionHelpers.CastToCollection(fieldInfo, instance, values);
            }
            else
            {
                if (!allowNonCollection) throw new InvalidOperationException("Cannot assign collection to non-collection field");
                fieldInfo.SetValue(instance, values.Cast<object>().FirstOrDefault());
            }
        }

        public static bool TryGetField(Type type, string fieldName, out FieldInfo fieldInfo)
        {
            fieldInfo = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!;
            return fieldInfo != null;
        }

        public static bool TryGetField(Type type, string[] fieldNames, out FieldInfo fieldInfo)
        {
            foreach (var name in fieldNames)
            {
                if (TryGetField(type, name, out fieldInfo)) return true;
            }
            fieldInfo = null!;
            return false;
        }

        public static string[] GetCommonFieldNames(string propName) =>
            [$"<{propName}>k__BackingField",
            $"_{char.ToLower(propName[0]) + propName[1..]}",
            $"{char.ToLower(propName[0]) + propName[1..]}",
            $"_{propName}"];
    }
}
