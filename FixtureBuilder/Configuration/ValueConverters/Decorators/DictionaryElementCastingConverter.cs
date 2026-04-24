using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Configuration.ValueConverters.Decorators
{
    internal class DictionaryElementCastingConverter : IValueConverter
    {
        private readonly IValueConverter _inner;

        public DictionaryElementCastingConverter(IValueConverter inner)
        {
            ArgumentNullException.ThrowIfNull(inner);

            _inner = inner;
        }

        public object? Convert(Type target, object value, IFixtureContext context)
        {
            if (target.IsDictionary()
                && target.IsGenericType
                && target.GetEnumerableElementType() != value.GetType().GetEnumerableElementType()
                && (value is IDictionary
                || value.GetType().GetEnumerableElementType()?.GetGenericTypeDefinitionOrDefault() == (typeof(KeyValuePair<,>))))
            {
                var (targetKeyType, targetValueType) = target.GetDictionaryEnumerableTypes();

                var castDictionary = CastDictionaryElements(targetKeyType!, targetValueType!, (IEnumerable)value, context);

                return _inner.Convert(target, castDictionary, context);
            }
            return _inner.Convert(target, value, context);
        }

        private static IEnumerable CastDictionaryElements(Type fieldKeyType, Type fieldValueType, IEnumerable values, IFixtureContext context)
        {
            var enumerator = values.GetEnumerator();
            if (enumerator.MoveNext())
            {
                var dict = (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(fieldKeyType, fieldValueType))!;
                var getter = MakeKeyValueGetter(enumerator.Current.GetType());
                do
                {
                    var (key, value) = getter(enumerator.Current);

                    key = ConvertElement(key, fieldKeyType, context);
                    value = ConvertElement(value, fieldValueType, context);

                    try
                    {
                        dict.Add(key!, value);
                    }
                    catch (ArgumentException ex) { throw new InvalidCastException(ex.Message); }
                } while (enumerator.MoveNext());
                values = dict;
            }
            return values;
        }

        private static object? ConvertElement(object? item, Type targetType, IFixtureContext context)
        {
            if (item == null) return null;
            if (targetType.IsAssignableFrom(item.GetType())) return item;

            var converted = context.Converter.Root.Convert(targetType, item, context);
            if (converted is NoResult)
                throw new InvalidCastException(
                    $"Cannot convert element of type {item.GetType().Name} to {targetType.Name}.");
            return converted;
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
