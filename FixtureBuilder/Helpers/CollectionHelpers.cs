using System.Reflection;

namespace FixtureBuilder.Helpers
{
    internal static class CollectionHelpers
    {
        public static void CastToCollection<T>(FieldInfo fieldInfo, object instance, T values)
        {
            if (values == null) throw new InvalidOperationException("Cannot cast null to a collection");
            if (values is not System.Collections.IEnumerable valuesCollection)
                throw new InvalidCastException("Values must be assignable to IEnumerable");

            var fieldType = fieldInfo.FieldType;

            if (!typeof(System.Collections.IEnumerable).IsAssignableFrom(fieldType))
                throw new InvalidOperationException("Cannot assign collection to a non-collection field");

            var emptyCollection = Activator.CreateInstance(fieldType);
            fieldInfo.SetValue(instance, emptyCollection);

            var addMethod = fieldType.GetMethod("Add")
                ?? throw new InvalidOperationException("Cannot assign collection to field without Add method");

            foreach (var item in valuesCollection)
                addMethod.Invoke(emptyCollection, [item]);
        }
    }
}
