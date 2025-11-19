using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace FixtureBuilder
{
    internal static class InstantiationHelpers
    {
        public static object GetInstantiatedInstance(Type type, bool instantiateMembers)
        {
            var instance = UseConstructor(type) ?? BypassConstructor(type)
                ?? throw new InvalidOperationException($"Failed to instantiate {type}");

            if (instantiateMembers) InstantiateMembers(instance);

            return instance;
        }

        public static object? BypassConstructor(Type type)
        {
            try { return RuntimeHelpers.GetUninitializedObject(type); }
            catch { return null; }
        }

        public static object? UseConstructor(Type type, params object[] args)
        {
            try
            {
                return Activator.CreateInstance(
                    type,
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                    binder: null,
                    args: args,
                    culture: CultureInfo.CurrentCulture);
            }
            catch { return null; }
        }

        public static void InstantiateMembers(object obj, HashSet<object>? seen = null)
        {
            if (obj == null) throw new InvalidOperationException($"Cannot instantiate members of a null member");
            seen ??= new HashSet<object>(ReferenceEqualityComparer.Instance);
            if (!seen.Add(obj)) return;

            var type = obj.GetType();

            var dataMembers = type.GetDataMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

            foreach (var dataMember in dataMembers)
            {
                if (dataMember.DeclaringType?.Namespace?.StartsWith("System") == true) continue;
                if (dataMember.DeclaringType?.Namespace?.StartsWith("Microsoft") == true) continue;
                if (dataMember.DeclaringType?.Namespace?.StartsWith("RunTime") == true) continue;
                if (dataMember.IsPropertyInfo)
                {
                    if (!dataMember.CanReadProperty || !dataMember.CanWriteProperty) continue;
                    if (dataMember.GetPropertyIndexParameters().Length > 0) continue;
                }
                if (dataMember.IsFieldInfo && dataMember.IsStaticField) continue;

                var dataMemberType = dataMember.DataMemberType;

                if (IsNullableReferenceType(dataMember) || IsNullableValueType(dataMemberType)) continue;
                if (dataMemberType.IsAbstract) continue;

                var value = dataMember.GetValue(obj);

                if (value == null)
                {
                    if (dataMemberType.IsPrimitive) value = default;
                    else if (dataMemberType == typeof(string)) value = "";
                    else if (dataMemberType == typeof(decimal)) value = 0m;
                    else if (dataMemberType.IsEnum) value = Enum.GetValues(dataMemberType).GetValue(0);
                    else if (dataMemberType.IsValueType) value = default;
                    else if (dataMemberType.IsClass || typeof(System.Collections.IEnumerable).IsAssignableFrom(dataMemberType))
                    {
                        try { value = GetInstantiatedInstance(dataMemberType, instantiateMembers: false); }
                        catch { }
                    }

                    if (value != null) dataMember.SetValue(obj, value);
                }

                if (value != null) InstantiateMembers(value, seen);
            }
        }

        private static bool IsNullableValueType(Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        private static bool IsNullableReferenceType(DataMemberInfo dataMember)
        {
            var attr = dataMember.CustomAttributes;

            // Check NullableAttribute on the field
            var nullableAttr = attr
                .FirstOrDefault(a => a.AttributeType.FullName == "System.Runtime.CompilerServices.NullableAttribute");

            if (nullableAttr != null && nullableAttr.ConstructorArguments.Count == 1)
            {
                var arg = nullableAttr.ConstructorArguments[0];

                if (arg.ArgumentType == typeof(byte[]) &&
                    arg.Value is ReadOnlyCollection<CustomAttributeTypedArgument> args &&
                    args.Count > 0 &&
                    args[0].Value is byte b &&
                    b == 2)
                {
                    return true; // Nullable reference type
                }
                else if (arg.ArgumentType == typeof(byte))
                {
                    if (arg.Value is byte by && by == 2)
                    {
                        return true; // Nullable reference type
                    }
                }
            }

            return false; // Default: not nullable reference type
        }
    }
}
