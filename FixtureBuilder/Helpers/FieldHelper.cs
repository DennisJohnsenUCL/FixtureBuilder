using System.Reflection;

namespace FixtureBuilder.Helpers
{
    internal static class FieldHelper
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

        public static bool TryGetPropertyBackingField(Type propertyParentType, PropertyInfo property, string? fieldName, out FieldInfo backingField)
        {
            var declaringType = property.DeclaringType;
            string[] fieldNames;

            if (fieldName == null)
            {
                fieldNames = GetCommonFieldNames(property.Name);

                if (declaringType != null && declaringType.IsInterface)
                {
                    var explicitFieldName = $"<{declaringType.FullName}.{property.Name}>k__BackingField";
                    fieldNames = [explicitFieldName, .. fieldNames];
                }
            }
            else fieldNames = [fieldName];

            if (TryGetField(propertyParentType, fieldNames, out backingField)) { }
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
