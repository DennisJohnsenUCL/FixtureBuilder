using System.Collections;
using System.Reflection;

namespace FixtureBuilder.Helpers
{
    internal static class CollectionHelpers
    {
        public static void CastToCollection(FieldInfo fieldInfo, object instance, IEnumerable values)
        {
            if (values == null) throw new InvalidOperationException("Cannot cast null to a collection");

            var fieldType = fieldInfo.FieldType;

            if (!typeof(IEnumerable).IsAssignableFrom(fieldType))
                throw new InvalidOperationException("Cannot assign collection to a non-collection field");

            var emptyCollection = Activator.CreateInstance(fieldType);
            fieldInfo.SetValue(instance, emptyCollection);

            var addMethod = fieldType.GetMethod("Add")
                ?? throw new InvalidOperationException("Cannot assign collection to field without Add method");

            foreach (var item in values)
                addMethod.Invoke(emptyCollection, [item]);
        }
    }
}
