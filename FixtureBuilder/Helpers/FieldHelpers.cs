using System.Reflection;

namespace FixtureBuilder.Helpers
{
    internal static class FieldHelpers
    {
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

        public static bool TryGetPropertyBackingField<TEntity>(PropertyInfo property, string? fieldName, out FieldInfo backingField)
        {
            var fieldNames = fieldName == null ? GetCommonFieldNames(property.Name) : [fieldName];
            var declaringType = property.DeclaringType;

            if (declaringType != null && declaringType.IsInterface)
            {
                var explicitFieldName = $"<{declaringType.FullName}.{property.Name}>k__BackingField";
                fieldNames = [explicitFieldName, .. fieldNames];
            }

            if (TryGetField(typeof(TEntity), fieldNames, out backingField)) { }
            else if (declaringType != null && TryGetField(declaringType, fieldNames, out backingField)) { }
            else return false;

            return true;
        }

        public static string[] GetCommonFieldNames(string propName) =>
            [$"<{propName}>k__BackingField",
            $"_{char.ToLower(propName[0]) + propName[1..]}",
            $"{char.ToLower(propName[0]) + propName[1..]}",
            $"_{propName}"];
    }
}
