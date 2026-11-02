using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;
using FixtureBuilder.Extensions;

namespace FixtureBuilder.Configuration.ValueConverters.Casters
{
    internal class DictionaryElementCaster : IDictionaryElementCaster
    {
        public object Cast(TypeRequest request, object value, IFixtureContext context)
        {
            var target = request.Type;
            if (target.IsDictionary()
                && target.IsGenericType
                && target.GetEnumerableElementType() != value.GetType().GetEnumerableElementType()
                && (value is IDictionary
                || value.GetType().GetEnumerableElementType()?.GetGenericTypeDefinitionOrDefault() == (typeof(KeyValuePair<,>))))
            {
                var (targetKeyType, targetValueType) = target.GetDictionaryEnumerableTypes();

                var castDictionary = CastDictionaryElements(targetKeyType!, targetValueType!, (IEnumerable)value, request.RootType, context);

                return castDictionary;
            }
            return value;
        }

        private static IEnumerable CastDictionaryElements(Type fieldKeyType, Type fieldValueType, IEnumerable values, Type rootType, IFixtureContext context)
        {
            var dict = (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(fieldKeyType, fieldValueType))!;
            Func<object, (object Key, object Value)>? getter = null;

            foreach (var item in values)
            {
                getter ??= MakeKeyValueGetter(item.GetType());
                var (key, value) = getter(item);

                key = ElementConverter.Convert(key, fieldKeyType, rootType, context);
                value = ElementConverter.Convert(value, fieldValueType, rootType, context);

                try
                {
                    dict.Add(key!, value);
                }
                catch (ArgumentException ex) { throw new InvalidCastException(ex.Message); }
            }
            return dict;
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
