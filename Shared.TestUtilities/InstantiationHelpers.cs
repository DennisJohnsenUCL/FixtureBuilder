using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Shared.TestUtilities
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

			foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
			{
				if (!prop.CanRead || !prop.CanWrite) continue;
				if (prop.GetIndexParameters().Length > 0) continue;
				if (IsNullableReferenceType(prop) || IsNullableValueType(prop)) continue;

				var propType = prop.PropertyType;

				if (propType.IsAbstract) continue;

				var value = prop.GetValue(obj);

				if (value == null)
				{
					if (propType.IsPrimitive) value = default;
					else if (propType == typeof(string)) value = "";
					else if (propType == typeof(decimal)) value = 0m;
					else if (propType.IsEnum) value = Enum.GetValues(propType).GetValue(0);
					else if (propType.IsValueType) value = default;
					else if (propType.IsClass || typeof(System.Collections.IEnumerable).IsAssignableFrom(propType))
					{
						try { value = GetInstantiatedInstance(propType, instantiateMembers: false); }
						catch { }
					}

					if (value != null) prop.SetValue(obj, value);
				}

				if (value != null) InstantiateMembers(value, seen);
			}

			foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
			{
				if (field.IsStatic) continue;
				if (field.DeclaringType?.Namespace?.StartsWith("System") == true) continue;
				if (field.DeclaringType?.Namespace?.StartsWith("Microsoft") == true) continue;
				if (field.DeclaringType?.Namespace?.StartsWith("RunTime") == true) continue;
				if (IsNullableReferenceType(field) || IsNullableValueType(field)) continue;

				var fieldType = field.FieldType;

				if (fieldType.IsAbstract) continue;

				var value = field.GetValue(obj);

				if (value == null)
				{
					if (fieldType.IsPrimitive) value = default;
					else if (fieldType == typeof(string)) value = "";
					else if (fieldType == typeof(decimal)) value = 0m;
					else if (fieldType.IsEnum) value = Enum.GetValues(fieldType).GetValue(0);
					else if (fieldType.IsValueType) value = default;
					else if (fieldType.IsClass || typeof(System.Collections.IEnumerable).IsAssignableFrom(fieldType))
					{
						try { value = GetInstantiatedInstance(fieldType, instantiateMembers: false); }
						catch { }
					}

					if (value != null) field.SetValue(obj, value);
				}

				if (value != null) InstantiateMembers(value, seen);
			}
		}

		private static bool IsNullableValueType(PropertyInfo property)
		{
			var type = property.PropertyType;

			return Nullable.GetUnderlyingType(type) != null;
		}

		private static bool IsNullableValueType(FieldInfo field)
		{
			var type = field.FieldType;

			return Nullable.GetUnderlyingType(type) != null;
		}

		private static bool IsNullableReferenceType(PropertyInfo property)
		{
			if (property.PropertyType.IsValueType)
			{
				// Value types can be Nullable<T> - handled elsewhere
				return false;
			}

			return IsNullableReferenceTypeInternal(property.CustomAttributes);
		}

		private static bool IsNullableReferenceType(FieldInfo field)
		{
			if (field.FieldType.IsValueType)
			{
				// Value types can be Nullable<T> - handled elsewhere
				return false;
			}

			return IsNullableReferenceTypeInternal(field.CustomAttributes);
		}

		private static bool IsNullableReferenceTypeInternal(IEnumerable<CustomAttributeData> attr)
		{
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
