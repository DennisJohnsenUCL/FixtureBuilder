using FixtureBuilder.Extensions;
using FixtureBuilder.Helpers;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace FixtureBuilder.ValueConverters.Decorators
{
    internal class DictionaryElementCastingConverter : IValueConverter
    {
        private readonly IValueConverter _inner;

        public DictionaryElementCastingConverter(IValueConverter inner)
        {
            ArgumentNullException.ThrowIfNull(inner);

            _inner = inner;
        }

        //TODO: Add check for either value IsDictionary or value implements IEnumerable<KeyValuePair<,>>, needs own method
        //Similar to value is IEnumerable for collections
        //This check catches everything that can be iterated with a key and value, avoids trying to cast something that cannot
        //TODO: Move IsDictionary to extension method on Type
        public object? Convert(Type target, object value)
        {
            if (CollectionHelpers.IsDictionary(target)
                && target.IsGenericType
                && target.GetEnumerableElementType() != value.GetType().GetEnumerableElementType())
            {
                var (targetKeyType, targetValueType) = GetTargetKeyValueTypes(target);

                var castDictionary = CastDictionaryElements(targetKeyType, targetValueType, (IEnumerable)value);

                return _inner.Convert(target, castDictionary);
            }
            return _inner.Convert(target, value);
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

        //TODO: This should only exist in one place
        private static (Type fieldKeyType, Type fieldValueType) GetTargetKeyValueTypes(Type fieldType)
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
