using System.Collections;

namespace FixtureBuilder.Helpers
{
    internal static class CollectionHelpers
    {
        public static IEnumerable CastToCollection(Type fieldType, IEnumerable values)
        {
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
