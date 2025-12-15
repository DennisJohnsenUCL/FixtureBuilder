using System.Collections;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Reflection;

namespace FixtureBuilder.Helpers
{
    internal static class CollectionHelpers
    {
        public static IEnumerable CastToCollection(Type fieldType, IEnumerable values)
        {
            if (fieldType.IsArray)
            {
                return CastToArray(fieldType, values);
            }

            else if (fieldType.IsInterface)
            {
                fieldType = GetConcreteType(fieldType);
            }

            if (fieldType.IsGenericType)
            {
                var genericTypeDef = fieldType.GetGenericTypeDefinition();

                if (genericTypeDef.FullName?.StartsWith("System.Collections.Immutable.Immutable") ?? false)
                {
                    var elementType = fieldType.GetGenericArguments()[0];
                    var typedList = CastElements(values, elementType);
                    return CastToImmutable(fieldType, genericTypeDef, elementType, typedList);
                }

                else if (genericTypeDef == typeof(ReadOnlyCollection<>))
                {
                    var elementType = fieldType.GetGenericArguments()[0];
                    var typedList = CastElements(values, elementType);
                    return CastToReadOnlyCollection(fieldType, elementType, typedList);
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

        private static Array CastToArray(Type fieldType, IEnumerable values)
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

        private static Type GetConcreteType(Type interfaceType)
        {
            Type concreteType;
            var genericTypeDef = interfaceType.GetGenericTypeDefinition();
            var elementType = interfaceType.GetGenericArguments()[0];

            if (genericTypeDef == typeof(IList<>) ||
                genericTypeDef == typeof(IReadOnlyList<>) ||
                genericTypeDef == typeof(IEnumerable<>) ||
                genericTypeDef == typeof(IReadOnlyCollection<>) ||
                genericTypeDef == typeof(ICollection<>))
            {
                concreteType = typeof(List<>);
            }
            else if (genericTypeDef == typeof(ISet<>) ||
                     genericTypeDef == typeof(IReadOnlySet<>))
            {
                concreteType = typeof(HashSet<>);
            }
            else if (genericTypeDef == typeof(IImmutableList<>)) concreteType = typeof(ImmutableList<>);
            else if (genericTypeDef == typeof(IImmutableSet<>)) concreteType = typeof(ImmutableHashSet<>);
            else throw new InvalidOperationException($"Unsupported interface type: {interfaceType.Name}");

            return concreteType.MakeGenericType(elementType);
        }

        private static IEnumerable CastToImmutable(Type fieldType, Type genericTypeDef, Type elementType, IEnumerable typedValues)
        {
            var factoryTypeName = genericTypeDef.FullName!.Replace("`1", "");
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

            return genericCreateRange.Invoke(null, [typedValues]) as IEnumerable
                ?? throw new InvalidOperationException($"Failed to create immutable collection for {fieldType.Name}");
        }

        private static IEnumerable CastToReadOnlyCollection(Type fieldType, Type elementType, IEnumerable typedValues)
        {
            var listType = typeof(List<>).MakeGenericType(elementType);
            var list = Activator.CreateInstance(listType);

            listType.GetMethod("AddRange")!.Invoke(list, [typedValues]);
            var readOnlyCollection = Activator.CreateInstance(fieldType, list)!;
            return (IEnumerable)readOnlyCollection;
        }

        private static IEnumerable CastElements(IEnumerable values, Type elementType)
        {
            var typedList = typeof(Enumerable)
                .GetMethod("Cast")!
                .MakeGenericMethod(elementType)
                .Invoke(null, [values]) as IEnumerable
                ?? throw new InvalidOperationException($"Failed to cast values to IEnumerable<{elementType.Name}>");
            return typedList;
        }
    }
}
