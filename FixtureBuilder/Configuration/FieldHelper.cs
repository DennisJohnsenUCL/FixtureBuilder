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
        /// Attempts to locate the backing field for a property, searching both the specified parent type
        /// and the property's declaring type.
        /// </summary>
        /// <param name="propertyParentType">The concrete type to search first for the backing field.</param>
        /// <param name="property">The property whose backing field to locate.</param>
        /// <param name="fieldName">
        /// An explicit field name to search for. If <see langword="null"/>, common naming conventions
        /// and compiler-generated backing field names are used as candidates.
        /// </param>
        /// <param name="backingField">
        /// When this method returns, contains the <see cref="FieldInfo"/> for the backing field
        /// if found; otherwise, <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if a backing field was found on either the parent type or the
        /// declaring type; otherwise, <see langword="false"/>.
        /// </returns>
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

        private static void ValidateNullableValueTypeAssignment(Type type, object? value)
        {
            if (value == null && type.IsValueType && !(type.GetGenericTypeDefinitionOrDefault() == typeof(Nullable<>)))
                throw new InvalidOperationException("Cannot assign null to a non-nullable value type. Consider passing default instead.");
        }

        /// <summary>
        /// Generates an array of common backing field name conventions for a given property name.
        /// </summary>
        /// <param name="propName">The property name to derive field names from.</param>
        /// <returns>
        /// An array of candidate field names in priority order: compiler-generated backing field,
        /// underscore-prefixed camelCase, camelCase, and underscore-prefixed original name.
        /// </returns>
        internal static string[] GetCommonFieldNames(string propName) =>
            [$"<{propName}>k__BackingField",
            $"_{char.ToLower(propName[0]) + propName[1..]}",
            $"{char.ToLower(propName[0]) + propName[1..]}",
            $"_{propName}"];
    }
}
