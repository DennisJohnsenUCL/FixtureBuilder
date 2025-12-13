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

        public static string[] GetCommonFieldNames(string propName) =>
            [$"<{propName}>k__BackingField",
            $"_{char.ToLower(propName[0]) + propName[1..]}",
            $"{char.ToLower(propName[0]) + propName[1..]}",
            $"_{propName}"];
    }
}
