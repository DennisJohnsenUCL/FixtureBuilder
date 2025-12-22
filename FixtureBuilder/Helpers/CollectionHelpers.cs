using System.Collections;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Reflection;

namespace FixtureBuilder.Helpers
{
    internal static class CollectionHelpers
    {
        public static IEnumerable CastToCollection(Type fieldType, IEnumerable values)
        {
            var sourceType = values.GetType();
            var sourceElementType = sourceType.IsGenericType ? sourceType.GenericTypeArguments[0] : null;

            if (sourceElementType != null && !ElementTypeIsAssignable(fieldType, sourceElementType))
                throw new InvalidOperationException($"Cannot assign type {sourceElementType.Name} as element in collection of type {fieldType.Name}.");

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
                var collection = InstantiationHelpers.UseConstructor(fieldType, (ICollection)values)
                    ?? throw new InvalidOperationException($"Failed to instantiate nongeneric collection: {fieldType.Name}.");

                return (IEnumerable)collection;
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

        private static bool ElementTypeIsAssignable(Type fieldType, Type elementType)
        {
            if (!typeof(IEnumerable).IsAssignableFrom(fieldType)) return false;

            Type? fieldElementType;
            if (fieldType.IsGenericType) fieldElementType = fieldType.GetGenericArguments()[0];
            else if (fieldType.IsArray) fieldElementType = fieldType.GetElementType();
            else return true;

            if (fieldElementType != null && (fieldElementType == elementType || fieldElementType.IsAssignableFrom(elementType))) return true;
            else return false;
        }

        public static IEnumerable CastToDictionary(Type fieldType, IEnumerable values)
        {
            //var keyValueTypes = values.GetType()
            //    .GetInterfaces()
            //    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            //    .Select(i => i.GetGenericArguments()[0])
            //    .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
            //    .Select(t => t.GetGenericArguments())
            //    .FirstOrDefault(args => args.Length == 2);

            //Type sourceKeyType = typeof(object);
            //Type sourceValueType = typeof(object);

            //if (keyValueTypes != null)
            //{
            //    sourceKeyType = keyValueTypes[0];
            //    sourceValueType = keyValueTypes[1];
            //}

            if (fieldType.IsInterface)
            {
                Type concreteType;

                if (fieldType.IsGenericType)
                {
                    var genericTypeDef = fieldType.GetGenericTypeDefinition();
                    var keyType = fieldType.GetGenericArguments()[0];
                    var valueType = fieldType.GetGenericArguments()[1];

                    if (genericTypeDef == typeof(IDictionary<,>)) concreteType = typeof(Dictionary<,>);
                    //        else if (genericTypeDef == typeof(IReadOnlyDictionary<,>)) concreteType = typeof(ReadOnlyDictionary<,>);
                    //        else if (genericTypeDef == typeof(IImmutableDictionary<,>)) concreteType = typeof(ImmutableDictionary<,>);
                    else throw new InvalidOperationException($"Unsupported generic dictionary interface type: {genericTypeDef.Name}");

                    fieldType = concreteType.MakeGenericType(keyType, valueType);
                }
                //    else if (fieldType == typeof(IDictionary))
                //    {
                //        fieldType = typeof(Dictionary<,>).MakeGenericType(sourceKeyType, sourceValueType);
                //    }
            }

            //Type fieldKeyType = typeof(object);
            //Type fieldValueType = typeof(object);

            if (fieldType.IsGenericType)
            {
                //var genericTypeDef = fieldType.GetGenericTypeDefinition();

                //var fieldGenArgs = fieldType.GetGenericArguments();

                //if (fieldGenArgs.Length == 2)
                //{
                //    fieldKeyType = fieldGenArgs[0];
                //    fieldValueType = fieldGenArgs[1];
                //}

                var dictionary = InstantiationHelpers.UseConstructor(fieldType, values);
                if (dictionary != null) return (IEnumerable)dictionary;

                //if (genericTypeDef == typeof(ImmutableDictionary<,>))
                //{
                //    var createRangeMethod = typeof(ImmutableDictionary)
                //        .GetMethods(BindingFlags.Public | BindingFlags.Static)
                //        .FirstOrDefault(m =>
                //            m.Name == "CreateRange" &&
                //            m.IsGenericMethodDefinition &&
                //            m.GetParameters().Length == 1)
                //        ?? throw new MissingMethodException("Did not find CreateRange method for ImmutableDictionary.");

                //    var genericCreateRange = createRangeMethod.MakeGenericMethod(fieldKeyType, fieldValueType);

                //    return (IEnumerable)genericCreateRange.Invoke(null, [values])!;
                //}

                //if (genericTypeDef == typeof(FrozenDictionary<,>))
                //{
                //    var ToFrozenDictionaryMethod = typeof(FrozenDictionary)
                //        .GetMethods(BindingFlags.Public | BindingFlags.Static)
                //        .FirstOrDefault(m =>
                //            m.Name == "ToFrozenDictionary" &&
                //            m.IsGenericMethodDefinition &&
                //            m.GetParameters().Length == 2)
                //        ?? throw new MissingMethodException("Did not find ToFrozenDictionary method for FrozenDictionary.");

                //    var genericToFrozenDictionary = ToFrozenDictionaryMethod.MakeGenericMethod(fieldKeyType, fieldValueType);

                //    return (IEnumerable)genericToFrozenDictionary.Invoke(null, [values, null])!;
                //}
            }
            throw new ApplicationException();
        }

        public static bool IsDictionary(Type fieldType)
        {
            var genericInterfaces = fieldType.GetInterfaces().Where(i => i.IsGenericType).Select(i => i.GetGenericTypeDefinition());
            if (genericInterfaces.Contains(typeof(IDictionary<,>)) || genericInterfaces.Contains(typeof(IReadOnlyDictionary<,>)))
                return true;

            else if (fieldType.IsGenericType)
            {
                var genericTypeDef = fieldType.GetGenericTypeDefinition();

                if (genericTypeDef == typeof(IDictionary<,>) || genericTypeDef == typeof(IReadOnlyDictionary<,>))
                    return true;
            }
            else if (typeof(IDictionary).IsAssignableFrom(fieldType)) return true;
            return false;
        }
    }
}
