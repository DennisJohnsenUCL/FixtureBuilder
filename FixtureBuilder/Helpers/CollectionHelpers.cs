using System.Collections;
using System.Reflection;

namespace FixtureBuilder.Helpers
{
    internal static class CollectionHelpers
    {
        public static IEnumerable CastToCollection(Type fieldType, IEnumerable values)
        {
            var valuesList = values.Cast<object>().ToList();

            if (fieldType.IsArray)
            {
                var elementType = fieldType.GetElementType()!;
                var array = Array.CreateInstance(elementType, valuesList.Count);
                for (int i = 0; i < valuesList.Count; i++)
                {
                    array.SetValue(valuesList[i], i);
                }
                return array;
            }

            if (fieldType.IsGenericType)
            {
                var genericTypeDef = fieldType.GetGenericTypeDefinition();
                var elementType = fieldType.GetGenericArguments()[0];

                if (genericTypeDef.FullName?.StartsWith("System.Collections.Immutable.Immutable") == true)
                {
                    var factoryTypeName = genericTypeDef.FullName.Replace("`1", "");
                    var factoryType = Type.GetType(factoryTypeName + ", System.Collections.Immutable")
                        ?? throw new InvalidOperationException($"Failed to resolve factory type for {fieldType.Name}");

                    var createRangeMethod = factoryType
                        .GetMethods(BindingFlags.Public | BindingFlags.Static)
                        .FirstOrDefault(m =>
                            m.Name == "CreateRange" &&
                            m.IsGenericMethodDefinition &&
                            m.GetParameters().Length == 1)
                        ?? throw new InvalidOperationException($"Failed to get CreateRange method for {fieldType.Name}");

                    var genericCreateRange = createRangeMethod.MakeGenericMethod(elementType);

                    var typedList = typeof(Enumerable)
                        .GetMethod("Cast")!
                        .MakeGenericMethod(elementType)
                        .Invoke(null, [valuesList]);

                    return genericCreateRange.Invoke(null, [typedList]) as IEnumerable
                        ?? throw new InvalidOperationException($"Failed to create immutable collection for {fieldType.Name}");
                }

                if (fieldType.IsInterface)
                {
                    Type concreteType;

                    if (genericTypeDef == typeof(IList<>) ||
                        genericTypeDef == typeof(IReadOnlyList<>) ||
                        genericTypeDef == typeof(IEnumerable<>) ||
                        genericTypeDef == typeof(IReadOnlyCollection<>) ||
                        genericTypeDef == typeof(ICollection<>))
                    {
                        concreteType = typeof(List<>).MakeGenericType(elementType);
                    }
                    else if (genericTypeDef == typeof(ISet<>) ||
                        genericTypeDef == typeof(IReadOnlySet<>))
                    {
                        concreteType = typeof(HashSet<>).MakeGenericType(elementType);
                    }
                    else throw new InvalidOperationException($"Unsupported interface type: {fieldType.Name}");

                    if (concreteType != null) fieldType = concreteType;
                }
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
