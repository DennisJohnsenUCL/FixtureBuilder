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
                var valuesList = values.Cast<object>().ToList();
                var elementType = fieldType.GetElementType()!;
                var array = Array.CreateInstance(elementType, valuesList.Count);
                for (int i = 0; i < valuesList.Count; i++)
                {
                    array.SetValue(valuesList[i], i);
                }
                return array;
            }

            if (fieldType.IsInterface)
            {
                Type concreteType;
                var genericTypeDef = fieldType.GetGenericTypeDefinition();
                var elementType = fieldType.GetGenericArguments()[0];

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
                else throw new InvalidOperationException($"Unsupported interface type: {fieldType.Name}");

                if (concreteType != null) fieldType = concreteType.MakeGenericType(elementType);
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
                        .Invoke(null, [values]);

                    return genericCreateRange.Invoke(null, [typedList]) as IEnumerable
                        ?? throw new InvalidOperationException($"Failed to create immutable collection for {fieldType.Name}");
                }
                else if (genericTypeDef == typeof(ReadOnlyCollection<>))
                {
                    var listType = typeof(List<>).MakeGenericType(elementType);
                    var list = Activator.CreateInstance(listType);

                    var typedList = typeof(Enumerable)
                        .GetMethod("Cast")!
                        .MakeGenericMethod(elementType)
                        .Invoke(null, [values]);

                    listType.GetMethod("AddRange")!.Invoke(list, [typedList]);
                    var readOnlyCollection = Activator.CreateInstance(fieldType, list)!;
                    return (IEnumerable)readOnlyCollection;
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
