using System.Collections;

namespace FixtureBuilder.Helpers
{
    internal static class CollectionHelpers
    {
        public static IEnumerable CastToCollection(Type fieldType, IEnumerable values)
        {
            if (fieldType.IsArray)
            {
                var valuesList = values.Cast<object>().ToList();
                var elementType = fieldType.GetElementType()!;
                var array = Array.CreateInstance(elementType, valuesList.Count);
                for (int i = 0; i < valuesList.Count; i++)
                {
                    array.SetValue(valuesList[i], i);
                }
                return array;
            }

            var collection = InstantiationHelpers.GetInstantiatedInstance(fieldType, instantiateMembers: false) as IEnumerable
                ?? throw new InvalidOperationException($"Failed to create collection instance for type {fieldType.Name}. Type must implement IEnumerable.");

            var addMethod = fieldType.GetMethod("Add")
                ?? throw new InvalidOperationException("Cannot assign collection to field without Add method");

            foreach (var item in values)
                addMethod.Invoke(collection, [item]);

            return collection;
        }
    }
}
