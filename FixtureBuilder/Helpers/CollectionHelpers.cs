using FixtureBuilder.Extensions;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
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
                fieldType = GetConcreteType(fieldType);
            }

            if (fieldType.TryGetGenericTypeDefinition(out var genericTypeDef))
            {
                var elementType = fieldType.GetGenericArguments()[0];
                var typedList = sourceElementType != null && sourceElementType == elementType
                    ? values
                    : CastElements(values, elementType);

                if (genericTypeDef == typeof(List<>) || genericTypeDef == typeof(Stack<>)
                    || genericTypeDef == typeof(Queue<>) || genericTypeDef == typeof(SortedSet<>)
                    || genericTypeDef == typeof(ReadOnlyCollection<>) || genericTypeDef == typeof(Collection<>)
                    || genericTypeDef == typeof(ConcurrentBag<>) || genericTypeDef == typeof(ConcurrentQueue<>)
                    || genericTypeDef == typeof(ConcurrentStack<>) || genericTypeDef == typeof(HashSet<>))
                {
                    var enumerable = InstantiationHelpers.UseConstructor(fieldType, typedList)!;
                    if (enumerable != null) return (IEnumerable)enumerable;
                }

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
                if (values is not ICollection) values = CastToArray(fieldType, values);

                var collection = InstantiationHelpers.UseConstructor(fieldType, values)
                    ?? throw new InvalidOperationException($"Failed to instantiate nongeneric collection: {fieldType.Name}.");

                return (IEnumerable)collection;
            }

            throw new InvalidOperationException($"Failed to cast to collection type: {fieldType.Name}");
        }

        private static Array CastToArray(Type fieldType, IEnumerable values)
        {
            var valuesList = values.Cast<object>().ToList();
            var elementType = fieldType.GetElementType() ?? typeof(object);
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

            if (interfaceType.TryGetGenericTypeDefinition(out var genericTypeDef))
            {
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
                else throw new InvalidOperationException($"Unsupported generic collection interface type: {interfaceType.Name}");

                return concreteType.MakeGenericType(elementType);
            }
            else
            {
                if (interfaceType == typeof(IList) ||
                    interfaceType == typeof(ICollection) ||
                    interfaceType == typeof(IEnumerable))
                {
                    return typeof(ArrayList);
                }
                else throw new InvalidOperationException($"Unsupported collection interface type: {interfaceType.Name}");
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
                .Invoke(null, [values])!;

            return (IEnumerable)typedList;
        }

        private static bool ElementTypeIsAssignable(Type fieldType, Type sourceElementType)
        {
            if (!typeof(IEnumerable).IsAssignableFrom(fieldType)) return false;

            Type? fieldElementType;
            if (fieldType.IsGenericType) fieldElementType = fieldType.GetGenericArguments()[0];
            else if (fieldType.IsArray) fieldElementType = fieldType.GetElementType();
            else return true;

            if (fieldElementType != null && (fieldElementType == sourceElementType || fieldElementType.IsAssignableFrom(sourceElementType))) return true;
            else return false;
        }

        public static IEnumerable CastToDictionary(Type fieldType, IEnumerable values)
        {
            if (fieldType.IsInterface)
            {
                fieldType = GetConcreteDictionaryType(fieldType);
            }

            if (fieldType.TryGetGenericTypeDefinition(out var genericTypeDef))
            {
                var (fieldKeyType, fieldValueType) = GetFieldKeyValueTypes(fieldType);
                var (sourceKeyType, sourceValueType) = GetSourceKeyValueTypes(values);

                if (fieldKeyType != sourceKeyType || fieldValueType != sourceValueType)
                {
                    values = CastDictionaryElements(fieldKeyType, fieldValueType, values);
                }

                if (genericTypeDef == typeof(Dictionary<,>)
                    || genericTypeDef == typeof(ConcurrentDictionary<,>)
                    || genericTypeDef == typeof(OrderedDictionary<,>))
                {
                    var dictionary = InstantiationHelpers.UseConstructor(fieldType, values);
                    if (dictionary != null) return (IEnumerable)dictionary;
                }

                else if (genericTypeDef.FullName?.StartsWith("System.Collections.Immutable.Immutable") ?? false)
                {
                    return CastToImmutableDictionary(fieldType, values);
                }

                else if (genericTypeDef == typeof(FrozenDictionary<,>))
                {
                    return CastToFrozenDictionary(fieldType, values);
                }

                else if (genericTypeDef == typeof(ReadOnlyDictionary<,>)
                    || genericTypeDef == typeof(SortedDictionary<,>)
                    || genericTypeDef == typeof(SortedList<,>))
                {
                    if (values is not IDictionary)
                    {
                        var iDictionaryType = typeof(Dictionary<,>).MakeGenericType(fieldKeyType, fieldValueType);
                        values = (IEnumerable)InstantiationHelpers.UseConstructor(iDictionaryType, values)!;
                    }
                    var readOnlyDictionary = InstantiationHelpers.UseConstructor(fieldType, values);
                    if (readOnlyDictionary != null) return (IEnumerable)readOnlyDictionary;
                    throw new InvalidOperationException("Failed to instantiate ReadOnlyDictionary.");
                }
            }

            else if (fieldType == typeof(SortedList) || fieldType == typeof(Hashtable))
            {
                return CastToNonGenericDictionary(fieldType, values);
            }
            throw new InvalidOperationException($"Failed to cast to dictionary type: {fieldType.Name}");
        }

        public static bool IsDictionary(Type fieldType)
        {
            if (typeof(IDictionary).IsAssignableFrom(fieldType)) return true;

            else if (fieldType.IsInterface && fieldType.TryGetGenericTypeDefinition(out var genericTypeDef))
            {
                if (genericTypeDef == typeof(IDictionary<,>) ||
                    genericTypeDef == typeof(IReadOnlyDictionary<,>) ||
                    genericTypeDef == typeof(IImmutableDictionary<,>))
                    return true;
            }

            return false;
        }

        private static IEnumerable CastDictionaryElements(Type fieldKeyType, Type fieldValueType, IEnumerable values)
        {
            var enumerator = values.GetEnumerator();
            if (enumerator.MoveNext())
            {
                var dict = (IDictionary)InstantiationHelpers.UseConstructor(typeof(Dictionary<,>).MakeGenericType(fieldKeyType, fieldValueType))!;
                var getter = MakeKeyValueGetter(enumerator.Current.GetType());
                do
                {
                    var (key, value) = getter(enumerator.Current);
                    dict.Add(key, value);
                } while (enumerator.MoveNext());
                values = dict;
            }
            return values;
        }

        private static Type GetConcreteDictionaryType(Type fieldType)
        {
            Type concreteType;

            if (fieldType.TryGetGenericTypeDefinition(out var genericTypeDef))
            {
                var keyType = fieldType.GetGenericArguments()[0];
                var valueType = fieldType.GetGenericArguments()[1];

                if (genericTypeDef == typeof(IDictionary<,>)) concreteType = typeof(Dictionary<,>);
                else if (genericTypeDef == typeof(IReadOnlyDictionary<,>)) concreteType = typeof(ReadOnlyDictionary<,>);
                else if (genericTypeDef == typeof(IImmutableDictionary<,>)) concreteType = typeof(ImmutableDictionary<,>);
                else throw new InvalidOperationException($"Unsupported generic dictionary interface type: {genericTypeDef.Name}");

                return concreteType.MakeGenericType(keyType, valueType);
            }
            else if (fieldType == typeof(IDictionary))
            {
                return typeof(Hashtable);
            }
            else throw new InvalidOperationException($"Unsupported dictionary interface type {fieldType.Name}");
        }

        private static (Type fieldKeyType, Type fieldValueType) GetFieldKeyValueTypes(Type fieldType)
        {
            Type fieldKeyType = typeof(object);
            Type fieldValueType = typeof(object);

            var fieldGenArgs = fieldType.GetGenericArguments();

            if (fieldGenArgs.Length == 2)
            {
                fieldKeyType = fieldGenArgs[0];
                fieldValueType = fieldGenArgs[1];
            }

            return (fieldKeyType, fieldValueType);
        }

        private static (Type sourceKeyType, Type sourceValueType) GetSourceKeyValueTypes(IEnumerable values)
        {
            Type sourceKeyType = typeof(object);
            Type sourceValueType = typeof(object);

            var keyValueTypes = values.GetType()
                .GetInterfaces()
                .Where(i => i.GetGenericTypeDefinitionOrDefault() == typeof(IEnumerable<>))
                .Select(i => i.GetGenericArguments()[0])
                .Where(t => t.GetGenericTypeDefinitionOrDefault() == typeof(KeyValuePair<,>))
                .Select(t => t.GetGenericArguments())
                .FirstOrDefault(args => args.Length == 2);

            if (keyValueTypes != null)
            {
                sourceKeyType = keyValueTypes[0];
                sourceValueType = keyValueTypes[1];
            }

            return (sourceKeyType, sourceValueType);
        }

        private static IEnumerable CastToImmutableDictionary(Type fieldType, IEnumerable values)
        {
            var genericTypeDef = fieldType.GetGenericTypeDefinition();

            var (fieldKeyType, fieldValueType) = GetFieldKeyValueTypes(fieldType);

            var factoryTypeName = genericTypeDef.FullName!.Replace("`2", "");
            var factoryType = Type.GetType(factoryTypeName + ", System.Collections.Immutable")
                ?? throw new InvalidOperationException($"Failed to resolve factory type for {fieldType.Name}.");

            var createRangeMethod = factoryType
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m =>
                    m.Name == "CreateRange" &&
                    m.IsGenericMethodDefinition &&
                    m.GetParameters().Length == 1)
                ?? throw new MissingMethodException("Did not find CreateRange method for ImmutableDictionary.");

            var genericCreateRange = createRangeMethod.MakeGenericMethod(fieldKeyType, fieldValueType);

            return (IEnumerable)genericCreateRange.Invoke(null, [values])!;
        }

        private static IEnumerable CastToFrozenDictionary(Type fieldType, IEnumerable values)
        {
            var (fieldKeyType, fieldValueType) = GetFieldKeyValueTypes(fieldType);

            var ToFrozenDictionaryMethod = typeof(FrozenDictionary)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(m =>
                    m.Name == "ToFrozenDictionary" &&
                    m.IsGenericMethodDefinition &&
                    m.GetParameters().Length == 2)
                ?? throw new MissingMethodException("Did not find ToFrozenDictionary method for FrozenDictionary.");

            var genericToFrozenDictionary = ToFrozenDictionaryMethod.MakeGenericMethod(fieldKeyType, fieldValueType);

            return (IEnumerable)genericToFrozenDictionary.Invoke(null, [values, null])!;
        }

        private static IEnumerable CastToNonGenericDictionary(Type fieldType, IEnumerable values)
        {
            if (values is not IDictionary)
            {
                var (sourceKeyType, sourceValueType) = GetSourceKeyValueTypes(values);
                var dictionaryType = typeof(Dictionary<,>).MakeGenericType(sourceKeyType, sourceValueType);

                values = (IEnumerable)InstantiationHelpers.UseConstructor(dictionaryType, values)!;
            }

            var nonGenericDictionary = InstantiationHelpers.UseConstructor(fieldType, values);
            if (nonGenericDictionary != null) return (IEnumerable)nonGenericDictionary;
            throw new InvalidOperationException($"Failed to cast to non-generic dictionary type: {fieldType.Name}");
        }

        private static Func<object, (object Key, object Value)> MakeKeyValueGetter(Type pairType)
        {
            // Parameter: object boxedItem
            var param = Expression.Parameter(typeof(object), "item");

            // Unbox (cast) to original type
            var cast = Expression.Convert(param, pairType);

            // Get Key property
            var keyProp = pairType.GetProperty("Key", BindingFlags.Instance | BindingFlags.Public);
            var valProp = pairType.GetProperty("Value", BindingFlags.Instance | BindingFlags.Public);

            if (keyProp == null || valProp == null)
                throw new ArgumentException("Type does not have Key/Value props");

            // Access properties
            var keyExpr = Expression.Property(cast, keyProp);
            var valExpr = Expression.Property(cast, valProp);

            // Box to object
            var keyBox = Expression.Convert(keyExpr, typeof(object));
            var valBox = Expression.Convert(valExpr, typeof(object));

            // Create tuple (object, object)
            var tuple = Expression.New(
                typeof(ValueTuple<object, object>).GetConstructor([typeof(object), typeof(object)])!,
                keyBox, valBox);

            // Lambda: object item => (object)item.Key, (object)item.Value
            var lambda = Expression.Lambda<Func<object, (object, object)>>(tuple, param);

            return lambda.Compile();
        }
    }
}
