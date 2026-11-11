using FixtureBuilder.Configuration.ValueConverters.Casters;
using FixtureBuilder.Core;
using FixtureBuilder.Core.FixtureContexts;

namespace FixtureBuilder.Configuration.ValueConverters
{
    /// <summary>
    /// Iterates through a sequence of <see cref="IValueConverter"/> instances, returning the first result that is not <see cref="NoResult"/>.
    /// Enumerable and dictionary elements are cast before any converter is tried.
    /// If that first result is not assignable to the original type, including <see langword="null"/> for a non-nullable value type, an <see cref="InvalidCastException"/> is thrown.
    /// Root-scoped converters are always tried before global and built-in ones, regardless of registration order.
    /// Returns <see cref="NoResult"/> if no converter handles the request.
    /// </summary>
    internal class CompositeConverter : ICompositeConverter
    {
        private readonly List<IValueConverter> _converters;
        private readonly List<IValueConverter> _rootConverters;
        private readonly IEnumerableElementCaster _enumerableElementCaster;
        private readonly IDictionaryElementCaster _dictionaryElementCaster;

        public CompositeConverter(IEnumerable<IValueConverter> converters, IEnumerableElementCaster enumerableElementCaster, IDictionaryElementCaster dictionaryElementCaster)
        {
            _converters = [.. converters];
            _rootConverters = [];
            _enumerableElementCaster = enumerableElementCaster;
            _dictionaryElementCaster = dictionaryElementCaster;
        }

        public object? Convert(TypeRequest request, object value, Type originalType, IFixtureContext context)
        {
            value = _enumerableElementCaster.Cast(request, value, context);
            value = _dictionaryElementCaster.Cast(request, value, context);

            foreach (var converter in _rootConverters.Concat(_converters))
            {
                var result = converter.Convert(request, value, context);

                if (result is NoResult) continue;

                if (result == null && originalType.IsValueType && Nullable.GetUnderlyingType(originalType) is null)
                    throw new InvalidCastException($"Converter returned null when converting {value.GetType().Name} to {originalType.Name}, which is a non-nullable value type.");

                if (result?.GetType().IsAssignableTo(originalType) == false)
                    throw new InvalidCastException($"Converter returned object of type {result.GetType().Name}, which is unassignable to target of type {originalType.Name}");

                return result;
            }
            return NoResult.Instance;
        }

        public void AddConverter(IValueConverter converter)
        {
            _converters.Insert(0, converter);
        }

        public void AddRootConverter(IValueConverter rootConverter)
        {
            _rootConverters.Insert(0, rootConverter);
        }
    }
}
