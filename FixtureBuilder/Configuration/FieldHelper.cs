using System.Globalization;
using System.Reflection;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Configuration
{
    /// <summary>
    /// Provides utility methods for locating fields on types using reflection,
    /// including support for common backing field naming conventions.
    /// </summary>
    internal static class FieldHelper
    {
        public static void SetFieldValue(FieldInfo fieldInfo, object instance, object? value)
        {
            var fieldType = fieldInfo.FieldType;

            ValidateNullableValueTypeAssignment(fieldType, value);

            var sourceType = value?.GetType();
            if (sourceType == null || fieldType == sourceType || fieldType.IsAssignableFrom(sourceType))
            {
                fieldInfo.SetValue(instance, value);
            }
            else throw new InvalidOperationException($"Cannot assign value of type {sourceType.Name} to field of type {fieldType.Name}");
        }

        public static void SetBackingFieldValue(FieldInfo backingField, PropertyInfo property, object instance, object? value, Type rootType, IFixtureContext context)
        {
            var fieldType = backingField.FieldType;

            ValidateNullableValueTypeAssignment(fieldType, value);

            var sourceType = value?.GetType();

            if (sourceType != null && fieldType != sourceType && !fieldType.IsAssignableFrom(sourceType))
            {
                var request = new FixtureRequest(fieldType, rootType);
                value = context.Converter.Root.Convert(request, value!, context);
                if (value is NoResult)
                    throw new InvalidOperationException($"Cannot assign type {sourceType.Name} to backing field for property {property.Name}, or convert it to a fitting type.");
            }

            backingField.SetValue(instance, value);
        }

        /// <summary>
        /// Attempts to retrieve a field from the specified type by checking multiple candidate field names in order.
        /// </summary>
        /// <param name="type">The type to search for the field.</param>
        /// <param name="fieldNames">An array of candidate field names to try, in order.</param>
        /// <param name="fieldInfo">
        /// When this method returns, contains the <see cref="FieldInfo"/> for the first matching field
        /// if found; otherwise, <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if any of the candidate field names matched a field on the type;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool TryGetField(Type type, string[] fieldNames, out FieldInfo fieldInfo)
        {
            foreach (var name in fieldNames)
            {
                if (type.TryGetField(name, out fieldInfo)) return true;
            }
            fieldInfo = null!;
            return false;
        }

        /// <summary>
        /// Attempts to locate the backing field for a property. The type hierarchy of <paramref name="propertyParentType"/>
        /// is walked to the most derived type declaring or overriding the property, and the field is searched for on that
        /// type and its base types. A property declared on an interface is first resolved to the class property implementing it.
        /// </summary>
        /// <param name="propertyParentType">The runtime type of the instance that owns the property.</param>
        /// <param name="property">The property whose backing field to locate.</param>
        /// <param name="fieldName">
        /// An explicit field name to search for. If <see langword="null"/>, the compiler-generated backing field name
        /// and common naming conventions are used as candidates.
        /// </param>
        /// <param name="backingField">
        /// When this method returns, contains the <see cref="FieldInfo"/> for the backing field
        /// if found; otherwise, <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a backing field was found; otherwise, <see langword="false"/>, including when
        /// <paramref name="propertyParentType"/> does not declare, inherit or implement the property.
        /// </returns>
        public static bool TryGetPropertyBackingField(Type propertyParentType, PropertyInfo property, string? fieldName, out FieldInfo backingField)
        {
            backingField = null!;

            var commonFieldNames = GetCommonFieldNames(property.Name);

            if (property.DeclaringType is { IsInterface: true })
            {
                if (GetPropertyImplementation(propertyParentType, property) is not { } implementation) return false;
                property = implementation;
            }

            string[] fieldNames = fieldName is not null ? [fieldName] : [GetAutoBackingFieldName(property), .. commonFieldNames];
            var getMethod = property.GetMethod?.GetBaseDefinition();

            var currentType = propertyParentType;
            while (currentType != null && currentType != typeof(object))
            {
                if (currentType.GetProperty(property.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly) is { } declaringProperty
                    && getMethod == declaringProperty.GetMethod?.GetBaseDefinition())
                {
                    break;
                }
                currentType = currentType.BaseType;
            }

            if (currentType == null || currentType == typeof(object)) return false;

            while (currentType != null && currentType != typeof(object))
            {
                if (TryGetField(currentType, fieldNames, out var fieldInfo))
                {
                    backingField = fieldInfo;
                    return true;
                }
                currentType = currentType.BaseType;
            }

            return false;
        }

        private static PropertyInfo? GetPropertyImplementation(Type propertyParentType, PropertyInfo interfaceProperty)
        {
            var iface = interfaceProperty.DeclaringType!;
            if (!iface.IsAssignableFrom(propertyParentType)) return null;

            var ifaceMap = propertyParentType.GetInterfaceMap(iface);
            var getterIndex = Array.FindIndex(ifaceMap.InterfaceMethods, m => m.HasSameMetadataDefinitionAs(interfaceProperty.GetMethod!));
            var target = ifaceMap.TargetMethods[getterIndex];

            return target.DeclaringType?
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                .First(p => p.GetMethod?.HasSameMetadataDefinitionAs(target) == true);
        }

        private static void ValidateNullableValueTypeAssignment(Type type, object? value)
        {
            if (value == null && type.IsValueType && !(type.GetGenericTypeDefinitionOrDefault() == typeof(Nullable<>)))
                throw new InvalidOperationException("Cannot assign null to a non-nullable value type. Consider passing default instead.");
        }

        internal static string GetAutoBackingFieldName(PropertyInfo propertyInfo)
        {
            return $"<{propertyInfo.Name}>k__BackingField";
        }

        /// <summary>
        /// Generates an array of common backing field name conventions for a given property name.
        /// </summary>
        /// <param name="propName">The property name to derive field names from.</param>
        /// <returns>
        /// An array of candidate field names in priority order:
        /// underscore-prefixed camelCase, camelCase, and underscore-prefixed original name.
        /// </returns>
        internal static string[] GetCommonFieldNames(string propName) =>
            [$"_{char.ToLower(propName[0], CultureInfo.InvariantCulture) + propName[1..]}",
            $"{char.ToLower(propName[0], CultureInfo.InvariantCulture) + propName[1..]}",
            $"_{propName}"];
    }
}
