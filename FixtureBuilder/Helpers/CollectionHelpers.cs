using System.Collections;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Reflection;

namespace FixtureBuilder.Helpers
{
    internal static class CollectionHelpers
    {
        public static IEnumerable CastToCollection(Type fieldType, IEnumerable values, Type? sourceElementType = null)
        {
            if (fieldType.IsInterface)
            {
                fieldType = GetConcreteType(fieldType, sourceElementType);
            }

            if (fieldType.IsGenericType)
            {
                var genericTypeDef = fieldType.GetGenericTypeDefinition();
                var elementType = fieldType.GetGenericArguments()[0];
                var typedList = sourceElementType != null && sourceElementType == elementType
                    ? values
                    : CastElements(values, elementType);

                var enumerable = InstantiationHelpers.UseConstructor(fieldType, typedList)!;
                if (enumerable != null) return (IEnumerable)enumerable;

                else if (genericTypeDef.FullName?.StartsWith("System.Collections.Immutable.Immutable") ?? false)
                {
                    return CastToImmutable(fieldType, genericTypeDef, typedList);
                }

                else if (genericTypeDef == typeof(FrozenSet<>))
                {
                    return CastToFrozenSet(fieldType, typedList);
                }
            }

            else if (fieldType.IsArray)
            {
                return CastToArray(fieldType, values);
            }

            else if (fieldType == typeof(ArrayList) || fieldType == typeof(Stack) || fieldType == typeof(Queue))
            {
                var arrayList = InstantiationHelpers.UseConstructor(fieldType, (ICollection)values)
                    ?? throw new InvalidOperationException($"Failed to instantiate nongeneric collection: {fieldType.Name}.");

                return (IEnumerable)arrayList;
            }

            throw new InvalidOperationException($"Failed to cast to collection type: {fieldType.Name}");
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

        private static Type GetConcreteType(Type interfaceType, Type? sourceElementType)
        {
            Type concreteType;

            if (interfaceType.IsGenericType)
            {
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
                else if (genericTypeDef == typeof(IImmutableStack<>)) concreteType = typeof(ImmutableStack<>);
                else if (genericTypeDef == typeof(IImmutableQueue<>)) concreteType = typeof(ImmutableQueue<>);
                else throw new InvalidOperationException($"Unsupported collection interface type: {interfaceType.Name}");

                return concreteType.MakeGenericType(elementType);
            }
            else
            {
                if (interfaceType == typeof(IList) ||
                    interfaceType == typeof(ICollection) ||
                    interfaceType == typeof(IEnumerable))
                {
                    concreteType = typeof(List<>);
                }
                else throw new InvalidOperationException($"Unsupported collection interface type: {interfaceType.Name}");

                if (sourceElementType == null) sourceElementType = typeof(object);

                return concreteType.MakeGenericType(sourceElementType);
            }
        }

        private static IEnumerable CastToImmutable(Type fieldType, Type genericTypeDef, IEnumerable values)
        {
            var elementType = fieldType.GetGenericArguments()[0];

            var factoryTypeName = genericTypeDef.FullName!.Replace("`1", "");
            var factoryType = Type.GetType(factoryTypeName + ", System.Collections.Immutable")
                ?? throw new InvalidOperationException($"Failed to resolve factory type for {fieldType.Name}.");

            var createRangeMethod = factoryType
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m =>
                    m.Name == "CreateRange" &&
                    m.IsGenericMethodDefinition &&
                    m.GetParameters().Length == 1)
                ?? throw new InvalidOperationException($"Failed to get CreateRange method for {fieldType.Name}.");

            var genericCreateRange = createRangeMethod.MakeGenericMethod(elementType);

            return genericCreateRange.Invoke(null, [values]) as IEnumerable
                ?? throw new InvalidOperationException($"Failed to create immutable collection for {fieldType.Name}.");
        }

        private static IEnumerable CastToFrozenSet(Type fieldType, IEnumerable values)
        {
            var elementType = fieldType.GetGenericArguments()[0];

            var ToFrozenSetMethod = typeof(FrozenSet)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m =>
                m.Name == "ToFrozenSet" &&
                m.IsGenericMethodDefinition &&
                m.GetParameters().Length == 2)
            ?? throw new InvalidOperationException($"Failed to get ToFrozenSet method for {fieldType.Name}.");

            var genericToFrozenSet = ToFrozenSetMethod.MakeGenericMethod(elementType);

            return genericToFrozenSet.Invoke(null, [values, null]) as IEnumerable
                ?? throw new InvalidOperationException($"Failed to create FrozenSet collection for {fieldType.Name}.");
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

        public static bool ElementTypeIsAssignable(Type fieldType, Type elementType)
        {
            if (!typeof(IEnumerable).IsAssignableFrom(fieldType)) return false;

            Type? fieldElementType;
            if (fieldType.IsGenericType) fieldElementType = fieldType.GetGenericArguments()[0];
            else if (fieldType.IsArray) fieldElementType = fieldType.GetElementType();
            else return true;

            if (fieldElementType != null && (fieldElementType == elementType || fieldElementType.IsAssignableFrom(elementType))) return true;
            else return false;
        }
    }
}
